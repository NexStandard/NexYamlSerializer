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

    public override EmitResult WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        WriteNewLine();
        return new EmitResult(machine.blockMapSecondaryKeySerializer);
    }

    public override EmitResult End(IEmitter currentEmitter)
    {
        return new EmitResult(currentEmitter);
        // Do nothing
    }
}
