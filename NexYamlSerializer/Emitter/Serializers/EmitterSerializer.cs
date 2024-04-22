using NexVYaml.Emitter;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
abstract class EmitterSerializer
{
    abstract public EmitState State { get; }
    abstract public void End();
    abstract public void Begin();
    abstract public void BeginScalar(Span<byte> output, ref int offset);
    abstract public void EndScalar(Span<byte> output, ref int offset);
}

