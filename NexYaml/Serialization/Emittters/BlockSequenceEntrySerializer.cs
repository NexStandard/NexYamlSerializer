using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockSequenceEntrySerializer : IEmitter
{
    public BlockSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.BlockSequenceEntry;

    public override void Begin()
    {
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteBlockSequenceEntryHeader();
                break;

            case EmitState.FlowSequenceEntry:
                throw new YamlException(
                    "To start block-sequence in the flow-sequence is not supported.");
            case EmitState.FlowMappingKey:
                throw new YamlException(
                    "To start block-sequence in the flow mapping key is not supported.");
            case EmitState.BlockMappingKey or EmitState.BlockMappingSecondaryKey:
                throw new YamlException(
                    "To start block-sequence in the mapping key is not supported.");
        }

        machine.Next = machine.Map(State);
        if (machine.TryGetTag(out var tag))
        {
            WriteRaw(tag);
            WriteNewLine();
        }
        switch (machine.Previous.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.IncreaseIndent();
                WriteNewLine();
                break;
            case EmitState.BlockMappingValue:
                WriteNewLine();
                break;
        }
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteSequenceSeparator();
        WriteRaw(output);
        WriteNewLine();
    }

    public override void End()
    {
        machine.PopState();
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;

            case EmitState.BlockMappingKey:
                throw new YamlException("Complex key is not supported.");

            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                break;
        }
    }
}
