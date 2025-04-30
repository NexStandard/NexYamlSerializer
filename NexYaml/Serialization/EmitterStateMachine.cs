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
    private IEmitter blockMapSecondaryKeySerializer;
    private IEmitter flowMapKeySerializer;
    private IEmitter flowMapSecondarySerializer;
    private IEmitter blockSequenceEntrySerializer;
    private IEmitter flowSequenceEntrySerializer;
    private IEmitter emptySerializer;
    private IEmitter blockMapValueSerializer;
    private IEmitter flowMapValueSerializer;
    private IEmitter flowSequenceSecondarySerializer;
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    public ExpandBuffer<IEmitter> StateStack { get; private set; } = new ExpandBuffer<IEmitter>(4);
    internal IndentationManager IndentationManager { get; } = new();
    protected ExpandBuffer<string> tagStack = new(4);
    public EmitterStateMachine(YamlWriter stream)
    {
        blockMapKeySerializer = new BlockMapKeySerializer(stream, this);
        blockMapSecondaryKeySerializer = new BlockMapSecondKeySerializer(stream, this);
        flowMapKeySerializer = new FlowMapKeySerializer(stream, this);
        flowMapSecondarySerializer = new FlowMapSecondKeySerializer(stream, this);
        blockSequenceEntrySerializer = new BlockSequenceEntrySerializer(stream, this);
        flowSequenceEntrySerializer = new FlowSequenceEntrySerializer(stream, this);
        blockMapValueSerializer = new BlockMapValueSerializer(stream, this);
        flowMapValueSerializer = new FlowMapValueSerializer(stream, this);
        flowSequenceSecondarySerializer = new FlowSequenceSecondaryEntrySerializer(stream, this);
        emptySerializer = new EmptySerializer(stream,this);
        if (StateStack.Length == 0)
        {
            StateStack.Add(emptySerializer);
        }
    }

    public IEmitter Map(EmitState state)
    {
        return state switch
        {
            EmitState.FlowMappingSecondaryKey => flowMapSecondarySerializer,
            EmitState.FlowSequenceSecondaryEntry => flowSequenceSecondarySerializer,
            EmitState.BlockMappingSecondaryKey => blockMapSecondaryKeySerializer,
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
    }

    public void Tag(ref string value)
    {
        tagStack.Add(value);
    }
    
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
        tagStack.Dispose();
    }
    public void WriteScalar(ReadOnlySpan<char> value)
    {
        StateStack.Current.WriteScalar(value);
    }
}
