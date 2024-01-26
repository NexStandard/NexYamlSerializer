using NexVYaml.Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class EmptySerializer : ISerializer
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
