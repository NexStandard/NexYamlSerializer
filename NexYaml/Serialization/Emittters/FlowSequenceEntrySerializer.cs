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
            case EmitState.BlockSequenceEntry:
            {
                WriteIndent();
                WriteSequenceSeparator();
                WriteFlowMappingStart();
                break;
            }
            case EmitState.FlowSequenceEntry:
            {
                if (!machine.IsFirstElement)
                {
                    WriteFlowSequenceSeparator();
                }
                WriteFlowSequenceStart();
                break;
            }
            case EmitState.BlockMappingValue:
            {
                if (!machine.IsFirstElement)
                {
                    WriteFlowSequenceSeparator();
                }
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
        if (machine.IsFirstElement)
        {
            if (machine.TryGetTag(out var tag))
            {
                WriteRaw(tag);
                WriteSpace();
            }
        }
        else
        {
            WriteFlowSequenceSeparator();
        }
        WriteRaw(value);
        machine.ElementCount++;
    }

    public override void End()
    {
        machine.PopState();

        var needsLineBreak = false;
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                machine.ElementCount++;
                break;
            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                needsLineBreak = true;
                machine.ElementCount++;
                break;
            case EmitState.FlowMappingValue:
                machine.Current = machine.Map(EmitState.FlowMappingKey);
                machine.ElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                machine.ElementCount++;
                break;
        }

        WriteFlowSequenceEnd();
        if (needsLineBreak)
        {
            WriteNewLine();
        }
    }
}
