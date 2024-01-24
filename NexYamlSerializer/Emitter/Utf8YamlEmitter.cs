#nullable enable
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexYamlSerializer.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;
using Stride.Engine;
namespace NexVYaml.Emitter
{
    public class YamlEmitterException(string message) : Exception(message)
    {
    }

    public enum EmitState
    {
        None,
        BlockSequenceEntry,
        BlockMappingKey,
        BlockMappingValue,
        FlowSequenceEntry,
        FlowMappingKey,
        FlowMappingValue,
    }
    public partial class Utf8YamlEmitter : IUtf8YamlEmitter
    {
        public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
        public ExpandBuffer<EmitState> StateStack { get; }
        public IBufferWriter<byte> Writer { get; }
        public YamlEmitOptions Options { get; }
        private ISerializer blockMapKeySerializer;
        private ISerializer flowMapKeySerializer;
        internal bool IsFirstElement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => currentElementCount <= 0;
        }

        internal IndentationManager IndentationManager { get; } = new();
        ExpandBuffer<int> elementCountStack;
        internal ExpandBuffer<string> tagStack;
        internal int currentElementCount;

        public Utf8YamlEmitter(IBufferWriter<byte> writer, YamlEmitOptions? options = null)
        {
            Writer = writer;
            Options = options ?? YamlEmitOptions.Default;

            StateStack = new ExpandBuffer<EmitState>(16);
            elementCountStack = new ExpandBuffer<int>(16);
            StateStack.Add(EmitState.None);
            currentElementCount = 0;
            blockMapKeySerializer = new BlockMapKeySerializer(this);
            flowMapKeySerializer = new FlowMapKeySerializer(this);
            tagStack = new ExpandBuffer<string>(4);
        }

        public void Dispose()
        {
            StateStack.Dispose();
            elementCountStack.Dispose();
            tagStack.Dispose();
        }

        void BeginBlockSequence()
        {
            switch (StateStack.Current)
            {
                case EmitState.BlockSequenceEntry:
                    WriteBlockSequenceEntryHeader();
                    break;

                case EmitState.FlowSequenceEntry:
                    throw new YamlEmitterException(
                        "To start block-sequence in the flow-sequence is not supported.");
                case EmitState.FlowMappingKey:
                    throw new YamlEmitterException(
                        "To start block-sequence in the flow mapping key is not supported.");
                case EmitState.BlockMappingKey:
                    throw new YamlEmitterException(
                        "To start block-sequence in the mapping key is not supported.");
            }

            PushState(EmitState.BlockSequenceEntry);
        }
        void BeginFlowSequence()
        {
            switch (StateStack.Current)
            {
                case EmitState.BlockMappingKey:
                    throw new YamlEmitterException("To start block-mapping in the mapping key is not supported.");
                case EmitState.FlowMappingKey:
                    throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");

                case EmitState.BlockSequenceEntry:
                    throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");
                case EmitState.FlowSequenceEntry:
                    {
                        break;
                    }
                case EmitState.BlockMappingValue:
                    break;
                default:
                    WriteRaw(YamlCodes.FlowSequenceStart);
                    break;
            }
            PushState(EmitState.FlowSequenceEntry);
        }
        public void BeginSequence(DataStyle style = DataStyle.Normal)
        {
            switch (style)
            {
                case DataStyle.Normal:
                    {
                        BeginBlockSequence();
                        break;
                    }
                case DataStyle.Compact:
                    {
                        BeginFlowSequence();
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }
        public void EndSequence(bool isEmpty)
        {
            switch (StateStack.Current)
            {
                case EmitState.BlockSequenceEntry:
                    {
                        var isEmptySequence = isEmpty;
                        PopState();

                        // Empty sequence
                        if (isEmptySequence)
                        {
                            var lineBreak = StateStack.Current is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
                            WriteRaw(EmitCodes.FlowSequenceEmpty, false, lineBreak);
                        }

                        switch (StateStack.Current)
                        {
                            case EmitState.BlockSequenceEntry:
                                if (!isEmptySequence)
                                {
                                    IndentationManager.DecreaseIndent();
                                }
                                currentElementCount++;
                                break;

                            case EmitState.BlockMappingKey:
                                throw new YamlEmitterException("Complex key is not supported.");

                            case EmitState.BlockMappingValue:
                                StateStack.Current = EmitState.BlockMappingKey;
                                currentElementCount++;
                                break;

                            case EmitState.FlowSequenceEntry:
                                currentElementCount++;
                                break;
                        }
                        break;
                    }
                case EmitState.FlowSequenceEntry:
                    {
                        PopState();

                        var needsLineBreak = false;
                        switch (StateStack.Current)
                        {
                            case EmitState.BlockSequenceEntry:
                                needsLineBreak = true;
                                currentElementCount++;
                                break;
                            case EmitState.BlockMappingValue:
                                StateStack.Current = EmitState.BlockMappingKey; // end mapping value
                                needsLineBreak = true;
                                currentElementCount++;
                                break;
                            case EmitState.FlowSequenceEntry:
                                currentElementCount++;
                                break;
                        }

                        var suffixLength = 1;
                        if (needsLineBreak) suffixLength++;

                        var offset = 0;
                        var output = Writer.GetSpan(suffixLength);
                        output[offset++] = YamlCodes.FlowSequenceEnd;
                        if (needsLineBreak)
                        {
                            output[offset++] = YamlCodes.Lf;
                        }
                        Writer.Advance(offset);
                        break;
                    }

                default:
                    throw new YamlEmitterException($"Current state is not sequence: {StateStack.Current}");
            }
        }
        public void BeginMapping(DataStyle style = DataStyle.Normal)
        {
            if (style is DataStyle.Normal)
                blockMapKeySerializer.Begin();
            else if (style is DataStyle.Compact)
                flowMapKeySerializer.Begin();
            else
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
        public void EndMapping()
        {
            if (StateStack.Current is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
            {
                throw new YamlEmitterException($"Invalid block mapping end: {StateStack.Current}");
            }
            if (StateStack.Current is EmitState.BlockMappingKey)
                blockMapKeySerializer.End();
            else if(StateStack.Current is EmitState.FlowMappingKey)
                flowMapKeySerializer.End();
        }

        public void Tag(string value)
        {
            tagStack.Add(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CalculateMaxScalarBufferLength(int length)
        {
            var around = (CurrentIndentLevel + 1) * Options.IndentWidth + 3;
            if (tagStack.Length > 0)
            {
                length += StringEncoding.Utf8.GetMaxByteCount(tagStack.Peek().Length) + around; // TODO:
            }
            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PushState(EmitState state)
        {
            StateStack.Add(state);
            elementCountStack.Add(currentElementCount);
            currentElementCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PopState()
        {
            StateStack.Pop();
            currentElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
        }
    }
}
