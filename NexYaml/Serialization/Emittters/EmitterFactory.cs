using NexYaml.Core;
using Stride.Core;

namespace NexYaml.Serialization.Emittters;
internal class EmitterFactory : IEmitterFactory
{
    private IEmitter blockMapKeySerializer;
    private IEmitter flowMapKeySerializer;
    private IEmitter blockSequenceEntrySerializer;
    private IEmitter flowSequenceEntrySerializer;
    private IEmitter emptySerializer;
    private IEmitter blockMapValueSerializer;
    private IEmitter flowMapValueSerializer;
    internal EmitterFactory(UTF8Stream emitter)
    {
        blockMapKeySerializer = new BlockMapKeySerializer(emitter);
        flowMapKeySerializer = new FlowMapKeySerializer(emitter);
        blockSequenceEntrySerializer = new BlockSequenceEntrySerializer(emitter);
        flowSequenceEntrySerializer = new FlowSequenceEntrySerializer(emitter);
        blockMapValueSerializer = new BlockMapValueSerializer(emitter);
        flowMapValueSerializer = new FlowMapValueSerializer(emitter);
        emptySerializer = new EmptySerializer();
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
}
