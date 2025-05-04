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
public class EmitterStateMachine
{
    internal IEmitter blockMapKeySerializer;
    internal IEmitter blockMapSecondaryKeySerializer;
    internal IEmitter flowMapKeySerializer;
    internal IEmitter flowMapSecondarySerializer;
    internal IEmitter blockSequenceEntrySerializer;
    internal IEmitter flowSequenceEntrySerializer;
    internal IEmitter emptySerializer;
    internal IEmitter blockMapValueSerializer;
    internal IEmitter flowMapValueSerializer;
    internal IEmitter flowSequenceSecondarySerializer;
    public int CurrentIndentLevel => IndentationManager.CurrentIndentLevel;
    public ExpandBuffer<IEmitter> StateStack { get; private set; } = new ExpandBuffer<IEmitter>(4);
    public StyleEnforcer StyleEnforcer { get; private set; }
    internal IndentationManager IndentationManager { get; } = new();
    public EmitterStateMachine(YamlWriter stream)
    {
        StyleEnforcer = stream.enforcer;
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

    public void PopState()
    {
        StateStack.Pop();
    }

    public void WriteScalar(ReadOnlySpan<char> value)
    {
        StateStack.Current = StateStack.Current.WriteScalar(value);
    }
}
