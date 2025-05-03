using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapValueSerializer : IEmitter
{
    public BlockMapValueSerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.BlockMappingValue;

    public override EmitResult Begin(BeginContext context)
    {
        throw new NotSupportedException();
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        WriteNewLine();
        machine.Current = machine.Map(EmitState.BlockMappingSecondaryKey);
    }

    public override void End()
    {
        throw new YamlException($"Can't end on {EmitState.BlockMappingValue.ToString()}");
        // Do nothing
    }
}
