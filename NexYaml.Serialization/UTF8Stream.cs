using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using System.Buffers;
using System.Text;
namespace NexYaml.Serialization;

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

public sealed class UTF8Stream : IUTF8Stream
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    internal ExpandBuffer<IEmitter> StateStack { get; private set; }
    public ArrayBufferWriter<char> Writer2 { get; } = new ArrayBufferWriter<char>();

    public int IndentWidth { get; } = 2;
    public IEmitterFactory EmitterFactory { get; private set; }

    internal bool IsFirstElement => currentElementCount <= 0;
    public SyntaxSettings settings { get; } = new();
    internal IndentationManager IndentationManager { get; } = new();

    private ExpandBuffer<int> elementCountStack;
    internal ExpandBuffer<string> tagStack;
    internal int currentElementCount;

    public UTF8Stream(SyntaxSettings settings = null)
    {
        if (settings is not null)
        {
            this.settings = settings;
        }
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
        if(StateStack is null)
        {
            StateStack = new ExpandBuffer<IEmitter>(4);

        }
        else
        {
            StateStack.Clear();
        }
        if(elementCountStack is null)
        {
            elementCountStack = new ExpandBuffer<int>(4);
        }
        else
        {
            elementCountStack.Clear();
        }
        if(EmitterFactory is null)
        {
            EmitterFactory = new EmitterFactory(this);
        }
        currentElementCount = 0;
        StateStack.Add(EmitterFactory.Map(EmitState.None));
        if(tagStack is null)
        {
            tagStack = new ExpandBuffer<string>(4);
        }
        else
        {
            tagStack.Clear();
        }
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
        WriteIndent();
         WriteRaw(settings.SequenceIdentifier);

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

    public ReadOnlyMemory<char> GetChars()
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
