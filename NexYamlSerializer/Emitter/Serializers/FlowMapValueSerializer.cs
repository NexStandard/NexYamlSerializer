using NexVYaml.Emitter;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapValueSerializer(Utf8YamlEmitter emitter) : EmitterSerializer
{
    public override EmitState State { get; } = EmitState.FlowMappingValue;

    public override void Begin()
    {
        throw new InvalidOperationException($"Can't start a {State} as Mapping");
    }

    public override void BeginScalar(Span<byte> output, ref int offset)
    {
        // Do Nothing
    }

    public override void End()
    {
        throw new NotImplementedException();
    }

    public override void EndScalar(Span<byte> output, ref int offset)
    {
        emitter.StateStack.Current = EmitState.FlowMappingKey;
        emitter.currentElementCount++;
    }
}
