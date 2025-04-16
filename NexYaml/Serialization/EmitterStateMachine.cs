using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using Stride.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization;
internal class EmitterStateMachine
{
    private IEmitter blockMapKeySerializer;
    private IEmitter flowMapKeySerializer;
    private IEmitter blockSequenceEntrySerializer;
    private IEmitter flowSequenceEntrySerializer;
    private IEmitter emptySerializer;
    private IEmitter blockMapValueSerializer;
    private IEmitter flowMapValueSerializer;
    public EmitterStateMachine(YamlWriter stream)
    {
        blockMapKeySerializer = new BlockMapKeySerializer(stream, this);
        flowMapKeySerializer = new FlowMapKeySerializer(stream, this);
        blockSequenceEntrySerializer = new BlockSequenceEntrySerializer(stream, this);
        flowSequenceEntrySerializer = new FlowSequenceEntrySerializer(stream, this);
        blockMapValueSerializer = new BlockMapValueSerializer(stream, this);
        flowMapValueSerializer = new FlowMapValueSerializer(stream, this);
        emptySerializer = new EmptySerializer();
        if (StateStack.Length == 0)
        {
            StateStack.Add(emptySerializer);
        }
    }

    public IEmitter Map(EmitState state)
    {
        return state switch
        {
            EmitState.BlockSequenceEntry => blockSequenceEntrySerializer,
            EmitState.FlowSequenceEntry => flowSequenceEntrySerializer,
            EmitState.FlowMappingKey => flowMapKeySerializer,
            EmitState.None => emptySerializer,
            EmitState.BlockMappingKey => blockMapKeySerializer,
            EmitState.BlockMappingValue => blockMapValueSerializer,
            EmitState.FlowMappingValue => flowMapValueSerializer,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public IEmitter BeginNodeMap(DataStyle style, bool isSequence)
    {
        if (isSequence)
        {

            if (style is DataStyle.Normal or DataStyle.Any)
            {
                return blockSequenceEntrySerializer;
            }
            else if (style is DataStyle.Compact)
            {
                return flowSequenceEntrySerializer;
            }
        }
        else
        {
            if (style is DataStyle.Normal or DataStyle.Any)
            {
                return blockMapKeySerializer;
            }
            else if (style is DataStyle.Compact)
            {
                return flowMapKeySerializer;
            }
        }
        throw new ArgumentException();
    }
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    public ExpandBuffer<IEmitter> StateStack { get; private set; } = new ExpandBuffer<IEmitter>(4);
    internal IndentationManager IndentationManager { get; } = new();

    protected ExpandBuffer<int> elementCountStack = new(4);
    protected ExpandBuffer<string> tagStack = new(4);
    public bool IsFirstElement => ElementCount == 0;
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
        ElementCount = 0;
        StateStack.Add(Map(EmitState.None));
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
    public void WriteScalar(ReadOnlySpan<char> value)
    {
        StateStack.Current.WriteScalar(value);
    }
}
