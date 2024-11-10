using NexVYaml.Emitter;
using System;

namespace NexYamlSerializer.Emitter.Serializers;
public interface IEmitter
{
    EmitState State { get; }
    void End();
    void Begin();
    void WriteScalar(ReadOnlySpan<char> value);
}