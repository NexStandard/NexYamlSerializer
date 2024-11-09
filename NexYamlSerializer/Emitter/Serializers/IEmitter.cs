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
    void WriteScalar(ReadOnlySpan<char> value);
}