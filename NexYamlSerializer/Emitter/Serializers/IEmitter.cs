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
    void BeginScalar(Span<byte> output);
    void EndScalar();
}

public static class IEmitterExtensions
{

}