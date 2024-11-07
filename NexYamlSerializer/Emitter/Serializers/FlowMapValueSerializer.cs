﻿using NexVYaml.Emitter;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapValueSerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowMappingValue;

    public void Begin()
    {
        throw new InvalidOperationException($"Can't start a {State} as Mapping");
    }

    public void BeginScalar(Span<byte> output)
    {
        // Do Nothing
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public void EndScalar()
    {
        emitter.Current = emitter.Map(EmitState.FlowMappingKey);
        emitter.currentElementCount++;
    }
}
