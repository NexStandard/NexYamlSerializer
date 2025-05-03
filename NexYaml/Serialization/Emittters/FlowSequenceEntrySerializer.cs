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

    public override void WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        machine.Current = machine.Map(EmitState.FlowSequenceSecondaryEntry);
    }

    public override void End()
    {
        machine.PopState();
        WriteFlowSequenceEnd();
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteNewLine();
                break;
            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                WriteNewLine();
                break;
            case EmitState.FlowMappingValue:
                machine.Current = machine.Map(EmitState.FlowMappingSecondaryKey);
                break;
            case EmitState.FlowSequenceEntry:
                break;
        }
    }
}
