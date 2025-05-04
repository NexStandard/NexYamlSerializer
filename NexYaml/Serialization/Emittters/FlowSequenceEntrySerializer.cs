using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowSequenceEntrySerializer : IEmitter
{
    public FlowSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.FlowSequenceEntry;

    public override EmitResult Begin(BeginContext context)
    {
        if (context.NeedsTag)
        {
            WriteRaw(context.Tag);
            WriteSpace();
        }
        switch (context.Emitter.State)
        {
            case EmitState.BlockMappingKey or EmitState.FlowMappingKey or EmitState.FlowMappingSecondaryKey or EmitState.BlockMappingSecondaryKey:
                throw new YamlException("To start flow-sequence in the mapping key is not supported.");
            default:
                WriteFlowSequenceStart();
            break;
        }
        return new EmitResult(this);
    }

    public override EmitResult WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        return new EmitResult(machine.flowSequenceSecondarySerializer);
    }

    public override EmitResult End(IEmitter currentEmitter)
    {
        WriteFlowSequenceEnd();
        switch (currentEmitter.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteNewLine();
                break;
            case EmitState.BlockMappingValue:
                WriteNewLine();
                return new EmitResult(machine.blockMapKeySerializer);
            case EmitState.FlowMappingValue:
                return new EmitResult(machine.flowMapSecondarySerializer);
            case EmitState.FlowSequenceEntry:
                break;
        }
        return new EmitResult(currentEmitter);
    }
}
