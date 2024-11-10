using System;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapValueSerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockMappingValue;

    public void Begin()
    {
        throw new NotImplementedException();
    }

    public void WriteScalar(ReadOnlySpan<char> output)
    {
        emitter.WriteRaw(output);
        emitter.WriteNewLine();
        emitter.Current = emitter.Map(EmitState.BlockMappingKey);
        emitter.currentElementCount++;
    }

    public void End()
    {
        // Do nothing
    }
}
