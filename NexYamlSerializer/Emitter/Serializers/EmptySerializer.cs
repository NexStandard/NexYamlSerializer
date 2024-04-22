using NexVYaml.Emitter;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class EmptySerializer : EmitterSerializer
{
    public static readonly EmptySerializer Instance = new();
    private EmptySerializer() { }
    public override EmitState State { get; } = EmitState.None;

    public override void Begin()
    {
    }

    public override void BeginScalar(Span<byte> output, ref int offset)
    {
    }

    public override void End()
    {
    }

    public override void EndScalar(Span<byte> output, ref int offset)
    {
    }
}
