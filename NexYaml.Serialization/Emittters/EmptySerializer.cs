﻿namespace NexYaml.Serialization.Emittters;
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
