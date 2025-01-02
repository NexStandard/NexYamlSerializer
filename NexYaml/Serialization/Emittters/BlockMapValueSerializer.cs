namespace NexYaml.Serialization.Emittters;
internal class BlockMapValueSerializer(IUTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockMappingValue;

    public void Begin()
    {
        throw new NotSupportedException();
    }

    public void WriteScalar(ReadOnlySpan<char> output)
    {
        emitter.WriteRaw(output);
        emitter.WriteNewLine();
        emitter.Current = emitter.Map(EmitState.BlockMappingKey);
        emitter.ElementCount++;
    }

    public void End()
    {
        // Do nothing
    }
}
