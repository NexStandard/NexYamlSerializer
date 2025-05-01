using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowSequenceEntrySerializer : IEmitter
{
    public FlowSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.FlowSequenceEntry;

    public override void Begin()
    {
        switch (machine.Current.State)
        {
            case EmitState.BlockMappingKey:
                throw new YamlException("To start block-mapping in the mapping key is not supported.");
            case EmitState.BlockMappingValue:
            {
                WriteFlowSequenceStart();
            }
            break;
            default:
                WriteFlowSequenceStart();
                break;
        }
        machine.Next = machine.Map(State);
    }

    public override void WriteScalar(ReadOnlySpan<char> value)
    {
        if (machine.TryGetTag(out var tag))
        {
            WriteRaw(tag);
            WriteSpace();
        }
        machine.Current = machine.Map(EmitState.FlowSequenceSecondaryEntry);
        WriteRaw(value);
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
