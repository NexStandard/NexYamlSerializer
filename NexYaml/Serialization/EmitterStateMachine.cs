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
    internal IndentationManager IndentationManager { get; } = new();
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
    }
}
