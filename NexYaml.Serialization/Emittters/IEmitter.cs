using System;

namespace NexYaml.Serialization.Emittters;
public interface IEmitter
{
    EmitState State { get; }
    void End();
    void Begin();
    void WriteScalar(ReadOnlySpan<char> value);
}