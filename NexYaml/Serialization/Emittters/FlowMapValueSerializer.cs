using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapValueSerializer : IEmitter
{
    public FlowMapValueSerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.FlowMappingValue;

    public override EmitResult Begin(BeginContext context)
    {
        throw new InvalidOperationException($"Can't start a {State} as Mapping");
    }

    public override void WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        machine.Current = machine.Map(EmitState.FlowMappingSecondaryKey);
    }

    public override void End()
    {
        throw new NotSupportedException();
    }
}
