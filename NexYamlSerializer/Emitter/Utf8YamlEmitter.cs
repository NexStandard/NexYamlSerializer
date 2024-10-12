#nullable enable
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;
using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
namespace NexVYaml.Emitter;

enum EmitState
{
    None,
    BlockSequenceEntry,
    BlockMappingKey,
    BlockMappingValue,
    FlowSequenceEntry,
    FlowMappingKey,
    FlowMappingValue,
}
sealed class Utf8YamlEmitter : IDisposable
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    internal ExpandBuffer<EmitState> StateStack { get; }
    internal ArrayBufferWriter<byte> Writer { get; } = new ArrayBufferWriter<byte>(512);

    public const int IndentWidth = 2;
    private EmitterSerializer blockMapKeySerializer;
    private EmitterSerializer flowMapKeySerializer;
    private EmitterSerializer blockSequenceEntrySerializer;
    private EmitterSerializer flowSequenceEntrySerializer;
    private EmitterSerializer emptySerializer;
    StyleEnforcer enforcer = new();
    internal bool IsFirstElement
    {
        get => currentElementCount <= 0;
    }

    internal IndentationManager IndentationManager { get; } = new();
    ExpandBuffer<int> elementCountStack;
    internal ExpandBuffer<string> tagStack;
    internal int currentElementCount;

    public Utf8YamlEmitter()
    {

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

    public void BeginSequence(DataStyle style)
    {
        enforcer.Begin(ref style);
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
        enforcer.End();
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
                throw new YamlException($"Current state is not sequence: {StateStack.Current}");
        }
    }

    public void BeginMapping(DataStyle style)
    {
        enforcer.Begin(ref style);
        if (style is DataStyle.Normal)
            blockMapKeySerializer.Begin();
        else if (style is DataStyle.Compact)
            flowMapKeySerializer.Begin();
        else
            blockMapKeySerializer.Begin();
    }

    public void EndMapping()
    {
        enforcer.End();
        if (StateStack.Current is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
        {
            throw new YamlException($"Invalid block mapping end: {StateStack.Current}");
        }
        if (StateStack.Current is EmitState.BlockMappingKey)
            blockMapKeySerializer.End();
        else if (StateStack.Current is EmitState.FlowMappingKey)
            flowMapKeySerializer.End();
    }

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
                emptySerializer.EndScalar(output, ref offset);
                break;
            default:
                throw new ArgumentOutOfRangeException(StateStack.Current.ToString());
        }
        Writer.Advance(offset);
    }
    #region Writes
    byte[] whiteSpaces =
[
        (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
    ];

    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        var offset = 0;
        var output = Writer.GetSpan(CalculateMaxScalarBufferLength(value.Length));

        BeginScalar(output, ref offset);
        value.CopyTo(output[offset..]);
        offset += value.Length;
        EndScalar(output, ref offset);
    }

    internal void WriteIndent(Span<byte> output, ref int offset, int forceWidth = -1)
    {
        int length;
        if (forceWidth > -1)
        {
            if (forceWidth <= 0)
                return;
            length = forceWidth;
        }
        else if (CurrentIndentLevel > 0)
        {
            length = CurrentIndentLevel * IndentWidth;
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

    internal void WriteRaw(byte value)
    {
        var output = Writer.GetSpan(1);
        output[0] = value;
        Writer.Advance(1);
    }

    internal void WriteRaw(ReadOnlySpan<byte> value, bool indent, bool lineBreak)
    {
        var length = value.Length +
                     (indent ? CurrentIndentLevel * IndentWidth : 0) +
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

    internal void WriteRaw(ReadOnlySpan<byte> value1, ReadOnlySpan<byte> value2, bool indent, bool lineBreak)
    {
        var length = value1.Length + value2.Length +
                     (indent ? CurrentIndentLevel * IndentWidth : 0) +
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

    internal void WriteBlockSequenceEntryHeader()
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
        WriteRaw(EmitCodes.BlockSequenceEntryHeader, true, false);
    }

    public int CalculateMaxScalarBufferLength(int length)
    {
        var around = ((CurrentIndentLevel + 1) * IndentWidth) + 3;
        if (tagStack.Length > 0)
        {
            length += StringEncoding.Utf8.GetMaxByteCount(tagStack.Peek().Length) + around;
        }
        return length;
    }

    internal void PushState(EmitState state)
    {
        StateStack.Add(state);
        elementCountStack.Add(currentElementCount);
        currentElementCount = 0;
    }

    internal void PopState()
    {
        StateStack.Pop();
        currentElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
    }
    public void Tag(ref string value)
    {
        tagStack.Add(value);
    }
    #endregion
}
