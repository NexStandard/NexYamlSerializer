using NexVYaml.Emitter;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class EmptySerializer : IEmitter
{
    public static readonly EmptySerializer Instance = new();
    private EmptySerializer() { }
    public EmitState State { get; } = EmitState.None;

    public void Begin()
    {
    }

    public void BeginScalar(Span<byte> output, ref int offset)
    {
    }

    public void End()
    {
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
    }
}
