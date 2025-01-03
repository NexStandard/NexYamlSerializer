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
        else if (current is EmitState.FlowSequenceEntry)
        {
            if (machine.ElementCount > 0)
            {
                WriteFlowSequenceSeparator();
            }
        }
        else if (current is EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
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
            WriteFlowMappingStart();
        }
        if (!machine.IsFirstElement)
        {
            WriteFlowSequenceSeparator();
        }
        WriteRaw(value);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.FlowMappingValue);
    }

    public override void End()
    {
        if (machine.Current.State is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
        {
            throw new YamlException($"Invalid block mapping end: {machine.Current}");
        }
        var isEmptyMapping = machine.IsFirstElement;
        var writeFlowMapEnd = true;
        if (isEmptyMapping)
        {
            if (machine.TryGetTag(out var tag))
            {
                WriteRaw(tag);
                WriteSpace();
                WriteEmptyFlowMapping();
                WriteNewLine();
                writeFlowMapEnd = false;
            }
            else
            {
                WriteEmptyFlowMapping();
                WriteNewLine();
                writeFlowMapEnd = false;
            }
        }
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
            case EmitState.FlowSequenceEntry:
                machine.ElementCount++;
                break;
            case EmitState.FlowMappingValue:
                machine.Current = machine.Map(EmitState.FlowMappingKey);
                machine.ElementCount++;
                break;
        }

        if (writeFlowMapEnd)
        {
            WriteFlowMappingEnd();
        }

        if (needsLineBreak)
        {
            WriteNewLine();
        }
    }
}
