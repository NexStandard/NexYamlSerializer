using NexVYaml.Emitter;
using Stride.Core;
using System;

namespace NexYamlSerializer.Emitter.Serializers;
internal class EmitterFactory : IEmitterFactory
{
    private IEmitter blockMapKeySerializer;
    private IEmitter flowMapKeySerializer;
    private IEmitter blockSequenceEntrySerializer;
    private IEmitter flowSequenceEntrySerializer;
    private IEmitter emptySerializer;
    private IEmitter blockMapValueSerializer;
    private IEmitter flowMapValueSerializer;
    internal EmitterFactory(IUTF8Stream emitter)
    {
        blockMapKeySerializer = new BlockMapKeySerializer((UTF8Stream)emitter);
        flowMapKeySerializer = new FlowMapKeySerializer((UTF8Stream)emitter);
        blockSequenceEntrySerializer = new BlockSequenceEntrySerializer((UTF8Stream)emitter);
        flowSequenceEntrySerializer = new FlowSequenceEntrySerializer((UTF8Stream)emitter);
        blockMapValueSerializer = new BlockMapValueSerializer((UTF8Stream)emitter);
        flowMapValueSerializer = new FlowMapValueSerializer((UTF8Stream)emitter);
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
