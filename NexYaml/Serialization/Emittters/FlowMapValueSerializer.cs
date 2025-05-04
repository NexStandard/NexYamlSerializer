using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapValueSerializer : IEmitter
{
    public FlowMapValueSerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.FlowMappingValue;

    public override IEmitter Begin(BeginContext context)
    {
        throw new InvalidOperationException($"Can't start a {State} as Mapping");
    }

    public override IEmitter WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        return machine.flowMapSecondarySerializer;
    }

    public override IEmitter End(IEmitter currentEmitter)
    {
        throw new YamlException($"Cant end on {State}");
    }
}
