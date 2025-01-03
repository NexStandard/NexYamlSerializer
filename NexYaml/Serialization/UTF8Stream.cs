using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using System.Buffers;

namespace NexYaml.Serialization;
public abstract class UTF8Stream : IDisposable, ICloneable
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    public ExpandBuffer<IEmitter> StateStack { get; private set; }
    public ArrayBufferWriter<char> Writer2 { get; } = new ArrayBufferWriter<char>();

    public int IndentWidth { get; } = 2;
    public IEmitterFactory EmitterFactory { get; private set; }

    public SyntaxSettings settings { get; } = new();
    public IndentationManager IndentationManager { get; } = new();

    protected ExpandBuffer<int> elementCountStack;
    protected ExpandBuffer<string> tagStack;

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
    public bool TryGetTag(out string tag)
    {
        return tagStack.TryPop(out tag);
    }
    public void PopState()
    {
        StateStack.Pop();
        ElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
    }

    public void Tag(ref string value)
    {
        tagStack.Add(value);
    }
    public int ElementCount { get; set; }
    public abstract void Flush();
    public abstract void WriteScalar(ReadOnlySpan<byte> value);
    public abstract void WriteRaw(ReadOnlySpan<byte> value);
    public abstract void WriteRaw(byte value);
    public virtual void Reset()
    {

        if (StateStack is null)
        {
            StateStack = new ExpandBuffer<IEmitter>(4);
        }
        else
        {
            StateStack.Clear();
        }
        if (elementCountStack is null)
        {
            elementCountStack = new ExpandBuffer<int>(4);
        }
        else
        {
            elementCountStack.Clear();
        }
        if (EmitterFactory is null)
        {
            EmitterFactory = new EmitterFactory(this);
        }
        ElementCount = 0;
        StateStack.Add(EmitterFactory.Map(EmitState.None));
        if (tagStack is null)
        {
            tagStack = new ExpandBuffer<string>(4);
        }
        else
        {
            tagStack.Clear();
        }
    }
    public virtual void Dispose()
    {
        StateStack.Dispose();
        elementCountStack.Dispose();
        tagStack.Dispose();
    }

    public abstract object Clone();
}