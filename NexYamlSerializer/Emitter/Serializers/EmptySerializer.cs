using NexVYaml.Emitter;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class EmptySerializer : IEmitter
{
    public EmitState State { get; } = EmitState.None;

    public void Begin()
    {
    }

    public void BeginScalar(Span<byte> output)
    {
    }

    public void End()
    {
    }

    public void EndScalar()
    {
    }
}
