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

    internal bool IsFirstElement => ElementCount <= 0;
    public SyntaxSettings settings { get; } = new();
    public IndentationManager IndentationManager { get; } = new();
    public int ElementCount { get; set; }
    private ExpandBuffer<int> elementCountStack;
    public ExpandBuffer<string> tagStack;


    public UTF8Stream(SyntaxSettings settings = null)
    {
        if (settings is not null)
        {
            this.settings = settings;
        }
        Reset();
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
        ElementCount = 0;
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

    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        // Create a span with enough capacity
        Span<char> span = stackalloc char[Encoding.UTF8.GetCharCount(value)];

        Encoding.UTF8.GetChars(value, span);
        StateStack.Current.WriteScalar(span);
    }

    public void WriteRaw(ReadOnlySpan<byte> value)
    {
        StringEncoding.Utf8.GetChars(value, Writer2);
    }

    public void WriteRaw(byte value)
    {
        StringEncoding.Utf8.GetChars([value], Writer2);
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
            elementCountStack.Add(ElementCount);
            ElementCount = 0;
        }
    }
    public IEmitter Previous => StateStack.Previous;

    public void PopState()
    {
        StateStack.Pop();
        ElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
    }

    public void Tag(ref string value)
    {
        tagStack.Add(value);
    }
    public bool TryGetTag(out string tag)
    {
        return tagStack.TryPop(out tag);
    }

    public ReadOnlyMemory<char> GetChars()
    {
        return Writer2.WrittenMemory;
    }
}
