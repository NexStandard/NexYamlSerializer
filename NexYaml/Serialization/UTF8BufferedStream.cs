using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization;
internal class UTF8BufferedStream : IUTF8Stream
{
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    internal ExpandBuffer<IEmitter> StateStack { get; private set; }
    public ArrayBufferWriter<char> Writer2 { get; } = new ArrayBufferWriter<char>();

    public int IndentWidth { get; } = 2;
    public IEmitterFactory EmitterFactory { get; private set; }

    public SyntaxSettings settings { get; } = new();
    public IndentationManager IndentationManager { get; } = new();

    private ExpandBuffer<int> elementCountStack;
    internal ExpandBuffer<string> tagStack;
    public int ElementCount { get; set; }
    BufferedStream stream;
    public UTF8BufferedStream(Stream stream)
    {
        this.stream = new BufferedStream(stream);
        Reset();
    }
    public void Dispose()
    {
    }
    public void Flush()
    {
        stream.Flush();
    }

    public ReadOnlyMemory<char> GetChars()
    {
        throw new NotSupportedException();
    }

    public void Reset()
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

    public void WriteRaw(ReadOnlySpan<byte> value)
    {
        stream.Write(value);
    }

    public void WriteRaw(byte value)
    {
        stream.Write([ value ]);
    }

    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        Span<char> span = stackalloc char[Encoding.UTF8.GetCharCount(value)];

        Encoding.UTF8.GetChars(value, span);
        StateStack.Current.WriteScalar(span);
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
}
