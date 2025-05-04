using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapValueSerializer : IEmitter
{
    public BlockMapValueSerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.BlockMappingValue;

    public override IEmitter Begin(BeginContext context)
    {
        throw new NotSupportedException();
    }

    public override IEmitter WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        WriteNewLine();
        return machine.blockMapSecondaryKeySerializer;
    }

    public override IEmitter End(IEmitter currentEmitter)
    {
        return currentEmitter;
        // Do nothing
    }
}
