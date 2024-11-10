using NexVYaml.Emitter;
using System;

namespace NexYamlSerializer.Emitter.Serializers;
internal class EmptySerializer : IEmitter
{
    public EmitState State { get; } = EmitState.None;

    public void Begin()
    {
    }

    public void WriteScalar(ReadOnlySpan<char> output)
    {
    }

    public void End()
    {
    }
}
