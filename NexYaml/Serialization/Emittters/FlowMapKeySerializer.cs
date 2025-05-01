using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapKeySerializer : IEmitter
{
    public FlowMapKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.FlowMappingKey;

    public override void Begin()
    {
        var current = machine.Current.State;
        if (current is EmitState.BlockSequenceEntry)
        {
            WriteBlockSequenceEntryHeader();
        }
        else if (current is EmitState.FlowSequenceSecondaryEntry)
        {
                 WriteFlowSequenceSeparator();
        }
        else if (current is EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
        }
        machine.Next = machine.Map(State);
        if (machine.TryGetTag(out var tag))
        {
            WriteRaw(tag);
            WriteSpace();
        }
        WriteFlowMappingStart();
    }

    public override void WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.FlowMappingValue);
    }

    public override void End()
    {
        machine.PopState();

        var needsLineBreak = false;
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                break;
            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                needsLineBreak = true;
                break;
            case EmitState.FlowSequenceEntry:
                machine.Current = machine.Map(EmitState.FlowSequenceSecondaryEntry);
                break;
            case EmitState.FlowMappingValue:
                machine.Current = machine.Map(EmitState.FlowMappingKey);
                break;
        }
        WriteFlowMappingEnd();

        if (needsLineBreak)
        {
            WriteNewLine();
        }
    }
}
