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
namespace NexVYaml.Emitter;

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
public sealed partial class Utf8YamlEmitter
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    internal ExpandBuffer<EmitState> StateStack { get; }
    public IBufferWriter<byte> Writer { get; }
    public YamlEmitOptions Options { get; }
    private IEmitter blockMapKeySerializer;
    private IEmitter flowMapKeySerializer;
    private IEmitter blockSequenceEntrySerializer;
    private IEmitter flowSequenceEntrySerializer;
    private IEmitter emptySerializer;
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
        blockSequenceEntrySerializer = new BlockSequenceEntrySerializer(this);
        flowSequenceEntrySerializer = new FlowSequenceEntrySerializer(this);
        emptySerializer = EmptySerializer.Instance;
        tagStack = new ExpandBuffer<string>(4);
    }

    public void Dispose()
    {
        StateStack.Dispose();
        elementCountStack.Dispose();
        tagStack.Dispose();
    }

    public void BeginSequence(DataStyle style = DataStyle.Normal)
    {
        switch (style)
        {
            case DataStyle.Normal or DataStyle.Any:
                {
                    blockSequenceEntrySerializer.Begin();
                    break;
                }
            case DataStyle.Compact:
                {
                    flowSequenceEntrySerializer.Begin();
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }
    public void EndSequence()
    {
        switch (StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                {
                    blockSequenceEntrySerializer.End();
                    break;
                }
            case EmitState.FlowSequenceEntry:
                {
                    flowSequenceEntrySerializer.End();
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
            throw new Exception($"Invalid block mapping end: {StateStack.Current}");
        }
        if (StateStack.Current is EmitState.BlockMappingKey)
            blockMapKeySerializer.End();
        else if(StateStack.Current is EmitState.FlowMappingKey)
            flowMapKeySerializer.End();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginScalar(Span<byte> output, ref int offset)
    {
        switch (StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                blockSequenceEntrySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.FlowSequenceEntry:
                flowSequenceEntrySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.BlockMappingKey:
                blockMapKeySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.BlockMappingValue:
                break;
            case EmitState.FlowMappingValue:
                break;
            case EmitState.FlowMappingKey:
                flowMapKeySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.None:
                emptySerializer.BeginScalar(output, ref offset);
                break;
            default:
                throw new ArgumentOutOfRangeException(StateStack.Current.ToString());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndScalar(Span<byte> output, ref int offset)
    {
        switch (StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                blockSequenceEntrySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.BlockMappingKey:
                blockMapKeySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.FlowMappingKey:
                flowMapKeySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.FlowMappingValue:
                StateStack.Current = EmitState.FlowMappingKey;
                currentElementCount++;
                break;
            case EmitState.BlockMappingValue:
                output[offset++] = YamlCodes.Lf;
                StateStack.Current = EmitState.BlockMappingKey;
                currentElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                flowSequenceEntrySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(StateStack.Current.ToString());
        }
        Writer.Advance(offset);
    }
}
