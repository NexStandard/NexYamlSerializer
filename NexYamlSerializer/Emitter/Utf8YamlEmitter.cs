#nullable enable
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using NexVYaml.Internal;
using NexYamlSerializer.Emitter;

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
    }
    public class Utf8YamlEmitter : IUtf8YamlEmitter
    {
        static byte[] whiteSpaces =
        {
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
        };
        static readonly byte[] BlockSequenceEntryHeader = { (byte)'-', (byte)' ' };
        static readonly byte[] FlowSequenceEmpty = { (byte)'[', (byte)']' };
        static readonly byte[] FlowSequenceSeparator = { (byte)',', (byte)' ' };
        static readonly byte[] MappingKeyFooter = { (byte)':', (byte)' ' };
        static readonly byte[] FlowMappingEmpty = { (byte)'{', (byte)'}' };

        public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
        public ExpandBuffer<EmitState> StateStack { get; }
        public IBufferWriter<byte> Writer { get; }
        public YamlEmitOptions Options { get; }

        bool IsFirstElement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => currentElementCount <= 0;
        }

        IndentationManager IndentationManager { get; } = new();
        ExpandBuffer<int> elementCountStack;
        ExpandBuffer<string> tagStack;
        int currentElementCount;

        public Utf8YamlEmitter(IBufferWriter<byte> writer, YamlEmitOptions? options = null)
        {
            Writer = writer;
            Options = options ?? YamlEmitOptions.Default;

            StateStack = new ExpandBuffer<EmitState>(16);
            elementCountStack = new ExpandBuffer<int>(16);
            StateStack.Add(EmitState.None);
            currentElementCount = 0;

            tagStack = new ExpandBuffer<string>(4);
        }

        public void Dispose()
        {
            StateStack.Dispose();
            elementCountStack.Dispose();
            tagStack.Dispose();
        }

        public void BeginSequence(SequenceStyle style = SequenceStyle.Block)
        {
            switch (style)
            {
                case SequenceStyle.Block:
                    {
                        switch (StateStack.Current)
                        {
                            case EmitState.BlockSequenceEntry:
                                WriteBlockSequenceEntryHeader();
                                break;

                            case EmitState.FlowSequenceEntry:
                                throw new YamlEmitterException(
                                    "To start block-sequence in the flow-sequence is not supported.");

                            case EmitState.BlockMappingKey:
                                throw new YamlEmitterException(
                                    "To start block-sequence in the mapping key is not supported.");
                        }

                        PushState(EmitState.BlockSequenceEntry);
                        break;
                    }
                case SequenceStyle.Flow:
                    {
                        switch (StateStack.Current)
                        {
                            case EmitState.BlockMappingKey:
                                throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");

                            case EmitState.BlockSequenceEntry:
                                {
                                    var output = Writer.GetSpan(CurrentIndentLevel * Options.IndentWidth + BlockSequenceEntryHeader.Length + 1);
                                    var offset = 0;
                                    WriteIndent(output, ref offset);
                                    BlockSequenceEntryHeader.CopyTo(output[offset..]);
                                    offset += BlockSequenceEntryHeader.Length;
                                    output[offset++] = YamlCodes.FlowSequenceStart;
                                    Writer.Advance(offset);
                                    break;
                                }
                            case EmitState.FlowSequenceEntry:
                                {
                                    var output = Writer.GetSpan(FlowSequenceSeparator.Length + 1);
                                    var offset = 0;
                                    FlowSequenceSeparator.CopyTo(output);
                                    offset += FlowSequenceSeparator.Length;
                                    output[offset++] = YamlCodes.FlowSequenceStart;
                                    Writer.Advance(offset);
                                    break;
                                }
                            default:
                                WriteRaw(YamlCodes.FlowSequenceStart);
                                break;
                        }
                        PushState(EmitState.FlowSequenceEntry);
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
                            WriteRaw(FlowSequenceEmpty, false, lineBreak);
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

        public void BeginMapping(MappingStyle style = MappingStyle.Block)
        {
            switch (style)
            {
                case MappingStyle.Block:
                    {
                        switch (StateStack.Current)
                        {
                            case EmitState.BlockMappingKey:
                                throw new YamlEmitterException("To start block-mapping in the mapping key is not supported.");

                            case EmitState.FlowSequenceEntry:
                                throw new YamlEmitterException("Cannot start block-mapping in the flow-sequence");

                            case EmitState.BlockSequenceEntry:
                                {
                                    WriteBlockSequenceEntryHeader();
                                    break;
                                }
                        }
                        PushState(EmitState.BlockMappingKey);
                        break;
                    }
                case MappingStyle.Flow:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        public void EndMapping()
        {
            if (StateStack.Current != EmitState.BlockMappingKey)
            {
                throw new YamlEmitterException($"Invalid block mapping end: {StateStack.Current}");
            }

            var isEmptyMapping = currentElementCount <= 0;
            PopState();

            if (isEmptyMapping)
            {
                var lineBreak = StateStack.Current is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
                if (tagStack.TryPop(out var tag))
                {
                    var tagBytes = StringEncoding.Utf8.GetBytes(tag + " "); // TODO:
                    WriteRaw(tagBytes, FlowMappingEmpty, false, lineBreak);
                }
                else
                {
                    WriteRaw(FlowMappingEmpty, false, lineBreak);
                }
            }

            switch (StateStack.Current)
            {
                case EmitState.BlockSequenceEntry:
                    if (!isEmptyMapping)
                    {
                        IndentationManager.DecreaseIndent();
                    }
                    currentElementCount++;
                    break;

                case EmitState.BlockMappingValue:
                    if (!isEmptyMapping)
                    {
                        IndentationManager.DecreaseIndent();
                    }
                    StateStack.Current = EmitState.BlockMappingKey;
                    currentElementCount++;
                    break;

                case EmitState.FlowSequenceEntry:
                    currentElementCount++;
                    break;
            }
        }

        public void Tag(string value)
        {
            tagStack.Add(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteScalar(ReadOnlySpan<byte> value)
        {
            var offset = 0;
            var output = Writer.GetSpan(CalculateMaxScalarBufferLength(value.Length));

            BeginScalar(output, ref offset);
            value.CopyTo(output[offset..]);
            offset += value.Length;
            EndScalar(output, ref offset);
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
        public void BeginScalar(Span<byte> output, ref int offset)
        {
            switch (StateStack.Current)
            {
                case EmitState.BlockSequenceEntry:
                    {
                        // first nested element
                        if (IsFirstElement)
                        {
                            switch (StateStack.Previous)
                            {
                                case EmitState.BlockSequenceEntry:
                                    IndentationManager.IncreaseIndent();
                                    output[offset++] = YamlCodes.Lf;
                                    break;
                                case EmitState.BlockMappingValue:
                                    output[offset++] = YamlCodes.Lf;
                                    break;
                            }
                        }
                        WriteIndent(output, ref offset);
                        BlockSequenceEntryHeader.CopyTo(output[offset..]);
                        offset += BlockSequenceEntryHeader.Length;

                        // Write tag
                        if (tagStack.TryPop(out var tag))
                        {
                            offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                            output[offset++] = YamlCodes.Lf;
                            WriteIndent(output, ref offset);
                        }
                        break;
                    }
                case EmitState.BlockMappingKey:
                    {
                        if (IsFirstElement)
                        {
                            switch (StateStack.Previous)
                            {
                                case EmitState.BlockSequenceEntry:
                                    {
                                        IndentationManager.IncreaseIndent();

                                        // Try write tag
                                        if (tagStack.TryPop(out var tag))
                                        {
                                            offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                                            output[offset++] = YamlCodes.Lf;
                                            WriteIndent(output, ref offset);
                                        }
                                        else
                                        {
                                            WriteIndent(output, ref offset, Options.IndentWidth - 2);
                                        }
                                        // The first key in block-sequence is like so that: "- key: .."
                                        break;
                                    }
                                case EmitState.BlockMappingValue:
                                    {
                                        IndentationManager.IncreaseIndent();
                                        // Try write tag
                                        if (tagStack.TryPop(out var tag))
                                        {
                                            offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                                        }
                                        output[offset++] = YamlCodes.Lf;
                                        WriteIndent(output, ref offset);
                                        break;
                                    }
                                default:
                                    WriteIndent(output, ref offset);
                                    break;
                            }

                            // Write tag
                            if (tagStack.TryPop(out var tag2))
                            {
                                offset += StringEncoding.Utf8.GetBytes(tag2, output[offset..]);
                                output[offset++] = YamlCodes.Lf;
                                WriteIndent(output, ref offset);
                            }
                        }
                        else
                        {
                            WriteIndent(output, ref offset);
                        }
                        break;
                    }
                case EmitState.BlockMappingValue:
                    break;

                case EmitState.FlowSequenceEntry:
                    if (currentElementCount > 0)
                    {
                        FlowSequenceSeparator.CopyTo(output[offset..]);
                        offset += FlowSequenceSeparator.Length;
                    }
                    break;
                case EmitState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndScalar(Span<byte> output, ref int offset)
        {
            switch (StateStack.Current)
            {
                case EmitState.BlockSequenceEntry:
                    output[offset++] = YamlCodes.Lf;
                    currentElementCount++;
                    break;
                case EmitState.BlockMappingKey:
                    MappingKeyFooter.CopyTo(output[offset..]);
                    offset += MappingKeyFooter.Length;
                    StateStack.Current = EmitState.BlockMappingValue;
                    break;
                case EmitState.BlockMappingValue:
                    output[offset++] = YamlCodes.Lf;
                    StateStack.Current = EmitState.BlockMappingKey;
                    currentElementCount++;
                    break;
                case EmitState.FlowSequenceEntry:
                    currentElementCount++;
                    break;
                case EmitState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteIndent(Span<byte> output, ref int offset, int forceWidth = -1)
        {
            int length;
            if (forceWidth > -1)
            {
                if (forceWidth <= 0) return;
                length = forceWidth;
            }
            else if (CurrentIndentLevel > 0)
            {
                length = CurrentIndentLevel * Options.IndentWidth;
            }
            else
            {
                return;
            }

            if (length > whiteSpaces.Length)
            {
                whiteSpaces = Enumerable.Repeat(YamlCodes.Space, length * 2).ToArray();
            }
            whiteSpaces.AsSpan(0, length).CopyTo(output[offset..]);
            offset += length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteRaw(byte value)
        {
            var output = Writer.GetSpan(1);
            output[0] = value;
            Writer.Advance(1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteRaw(ReadOnlySpan<byte> value, bool indent, bool lineBreak)
        {
            var length = value.Length +
                         (indent ? CurrentIndentLevel * Options.IndentWidth : 0) +
                         (lineBreak ? 1 : 0);

            var offset = 0;
            var output = Writer.GetSpan(length);
            if (indent)
            {
                WriteIndent(output, ref offset);
            }
            value.CopyTo(output[offset..]);
            if (lineBreak)
            {
                output[length - 1] = YamlCodes.Lf;
            }
            Writer.Advance(length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteRaw(ReadOnlySpan<byte> value1, ReadOnlySpan<byte> value2, bool indent, bool lineBreak)
        {
            var length = value1.Length + value2.Length +
                         (indent ? CurrentIndentLevel * Options.IndentWidth : 0) +
                         (lineBreak ? 1 : 0);
            var offset = 0;
            var output = Writer.GetSpan(length);
            if (indent)
            {
                WriteIndent(output, ref offset);
            }

            value1.CopyTo(output[offset..]);
            offset += value1.Length;

            value2.CopyTo(output[offset..]);
            if (lineBreak)
            {
                output[length - 1] = YamlCodes.Lf;
            }
            Writer.Advance(length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteBlockSequenceEntryHeader()
        {
            if (IsFirstElement)
            {
                switch (StateStack.Previous)
                {
                    case EmitState.BlockSequenceEntry:
                        WriteRaw(YamlCodes.Lf);
                        IndentationManager.IncreaseIndent();
                        break;
                    case EmitState.BlockMappingValue:
                        WriteRaw(YamlCodes.Lf);
                        break;
                }
            }
            WriteRaw(BlockSequenceEntryHeader, true, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushState(EmitState state)
        {
            StateStack.Add(state);
            elementCountStack.Add(currentElementCount);
            currentElementCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PopState()
        {
            StateStack.Pop();
            currentElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
        }
    }
}
