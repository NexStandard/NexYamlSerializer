using NexVYaml.Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockSequenceEntrySerializer : ISerializer
{
    public EmitState State { get; }

    public void Begin()
    {
        throw new NotImplementedException();
    }

    public void BeginScalar(Span<byte> output, ref int offset)
    {
        throw new NotImplementedException();
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
        throw new NotImplementedException();
    }
}
