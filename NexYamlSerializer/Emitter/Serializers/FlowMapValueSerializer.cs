using NexVYaml.Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapValueSerializer(Utf8YamlEmitter emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowMappingValue;

    public void Begin()
    {
        throw new InvalidOperationException($"Can't start a {State} as Mapping");
    }

    public void BeginScalar(Span<byte> output, ref int offset)
    {
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
        emitter.StateStack.Current = EmitState.FlowMappingKey;
        emitter.currentElementCount++;
    }
}
