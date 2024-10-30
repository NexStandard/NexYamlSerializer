using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
public interface IEmitter
{
    EmitState State { get; }
    void End();
    void Begin();
    void BeginScalar(Span<byte> output, ref int offset);
    void EndScalar(Span<byte> output, ref int offset);
}

public static class IEmitterExtensions
{

}