using NexYaml.Core;
using NexYamlSerializer.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using System;
using System.Buffers;
using System.Linq;
using System.Text;
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

internal sealed class UTF8Stream : IUTF8Stream
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    internal ExpandBuffer<IEmitter> StateStack { get; private set; }
    public ArrayBufferWriter<char> Writer2 { get; } = new ArrayBufferWriter<char>();

    public const int IndentWidth = 2;
    public IEmitterFactory EmitterFactory { get; private set; }


    internal bool IsFirstElement => currentElementCount <= 0;

    internal IndentationManager IndentationManager { get; } = new();

    private ExpandBuffer<int> elementCountStack;
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
        StateStack = new ExpandBuffer<IEmitter>(4);
        elementCountStack = new ExpandBuffer<int>(4);
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

    public IUTF8Stream WriteScalar(ReadOnlySpan<byte> value)
    {
        // Create a span with enough capacity
        Span<char> span = stackalloc char[Encoding.UTF8.GetCharCount(value)];

        Encoding.UTF8.GetChars(value, span);
        StateStack.Current.WriteScalar(span);
        return this;
    }
    public IUTF8Stream WriteScalar(string value)
    {
        StateStack.Current.WriteScalar(value);
        return this;
    }
    public IUTF8Stream WriteIndent(int forceWidth = -1)
    {
        int length;

        if (forceWidth > -1)
        {
            if (forceWidth <= 0)
                return this;
            length = forceWidth;
        }
        else if (CurrentIndentLevel > 0)
        {
            length = CurrentIndentLevel * IndentWidth;
        }
        else
        {
            return this;
        }
        Span<char> whiteSpaces = stackalloc char[]
        {
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '
        };

        if (length > whiteSpaces.Length)
        {
            whiteSpaces = Enumerable.Repeat(' ', length * 2).ToArray();
        }
        Writer2.Write(whiteSpaces.Slice(0, length));
        return this;
    }
    public IUTF8Stream WriteRaw(string? value)
    {
        Writer2.Write(value);
        return this;
    }
    public IUTF8Stream WriteRaw(ReadOnlySpan<byte> value)
    {
        StringEncoding.Utf8.GetChars(value, Writer2);
        return this;
    }

    internal IUTF8Stream WriteRaw(byte value)
    {
        StringEncoding.Utf8.GetChars([value], Writer2);
        return this;
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
        WriteRaw([(byte)'-', (byte)' ']);
        WriteIndent();
    }

    public IEmitter Current
    {
        get => StateStack.Current;
        set => StateStack.Current = value;
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
    public IEmitter Previous => StateStack.Previous;
    public void PopState()
    {
        StateStack.Pop();
        currentElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
    }
    public IUTF8Stream Tag(ref string value)
    {
        tagStack.Add(value);
        return this;
    }

    internal ReadOnlyMemory<char> GetBytes()
    {
        return Writer2.WrittenMemory;
    }

    public IUTF8Stream WriteRaw(ReadOnlySpan<char> value)
    {
        Writer2.Write(value);
        return this;
    }

    public IUTF8Stream WriteScalar(ReadOnlySpan<char> value)
    {
        StateStack.Current.WriteScalar(value);
        return this;
    }
}
