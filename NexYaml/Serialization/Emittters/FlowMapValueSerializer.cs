namespace NexYaml.Serialization.Emittters;
internal class FlowMapValueSerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowMappingValue;

    public void Begin()
    {
        throw new InvalidOperationException($"Can't start a {State} as Mapping");
    }

    public void WriteScalar(ReadOnlySpan<char> value)
    {
        emitter.WriteRaw(value);
        emitter.Current = emitter.Map(EmitState.FlowMappingKey);
        emitter.currentElementCount++;
    }

    public void End()
    {
        throw new NotImplementedException();
    }
}
