#nullable enable
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;
using System;
using System.Buffers;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
namespace NexVYaml.Emitter;

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
sealed class UTF8Stream : IUTF8Stream
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    internal ExpandBuffer<IEmitter> StateStack { get; private set; }
    public ArrayBufferWriter<byte> Writer { get; } = new ArrayBufferWriter<byte>(512);

    public const int IndentWidth = 2;
    public IEmitterFactory EmitterFactory { get; private set; }
    byte[] whiteSpaces =
    [
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
    ];

    internal bool IsFirstElement
    {
        get => currentElementCount <= 0;
    }

    internal IndentationManager IndentationManager { get; } = new();
    ExpandBuffer<int> elementCountStack;
    internal ExpandBuffer<string> tagStack;
    internal int currentElementCount;

    public UTF8Stream()
    {
        Reset();
    }
    public Span<char> TryRemoveDuplicateLineBreak(Span<char> scalarChars)
    {
        if (StateStack.Current.State is EmitState.BlockMappingValue or EmitState.BlockSequenceEntry)
        {
            scalarChars = scalarChars[..^1];
        }

        return scalarChars;
    }
    public void Reset()
    {
        StateStack = new ExpandBuffer<IEmitter>(16);
        elementCountStack = new ExpandBuffer<int>(16);
        EmitterFactory = new EmitterFactory(this);
        currentElementCount = 0;
        StateStack.Add(EmitterFactory.Map(EmitState.None));
        tagStack = new ExpandBuffer<string>(4);
    }

    public void Dispose()
    {
        StateStack.Dispose();
        elementCountStack.Dispose();
        tagStack.Dispose();
    }

    public void BeginScalar(Span<byte> output)
    {
        StateStack.Current.BeginScalar(output);
    }

    public void EndScalar()
    {

        StateStack.Current.EndScalar();
    }

    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        
        var output = Writer.GetSpan(CalculateMaxScalarBufferLength(value.Length));

        BeginScalar(output);
        Writer.Write(value);
        EndScalar();
    }
    public void WriteIndent(int forceWidth= -1)
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
        Writer.Write(whiteSpaces.AsSpan(0, length));
    }
    public void WriteIndent(Span<byte> output, ref int offset, int forceWidth = -1)
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

    public void WriteRaw(ReadOnlySpan<byte> value)
    {
        Writer.Write(value);
    }
    internal void WriteRaw(byte value)
    {
        Writer.Write([value]);
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
            WriteIndent();
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
            WriteIndent();
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
            switch (StateStack.Previous.State)
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
        WriteRaw([(byte)'-', (byte)' '], true, false);
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

    public IEmitter Current
    {
        get
        {
            return StateStack.Current;
        }
        set
        {
            StateStack.Current = value;
        }
    }
    public IEmitter Next
    {
        set
        {
            StateStack.Add(value);
            elementCountStack.Add(currentElementCount);
            currentElementCount = 0;
        }
    }
    public IEmitter Previous { get { return StateStack.Previous; } }
    public void PopState()
    {
        StateStack.Pop();
        currentElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
    }
    public void Tag(ref string value)
    {
        tagStack.Add(value);
    }
}
