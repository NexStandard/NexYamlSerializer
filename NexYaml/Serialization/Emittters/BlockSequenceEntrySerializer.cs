﻿using NexYaml.Core;

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
            case EmitState.BlockMappingKey:
                throw new YamlException(
                    "To start block-sequence in the mapping key is not supported.");
        }

        machine.Next = machine.Map(State);
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        // first nested element
        if (machine.IsFirstElement)
        {
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

        WriteIndent();
        WriteSequenceSeparator();

        // Write tag
        if (machine.TryGetTag(out var tag))
        {
            WriteRaw(tag);
            WriteNewLine();
            WriteIndent();
        }
        WriteRaw(output);
        WriteNewLine();

        machine.ElementCount++;
    }

    public override void End()
    {
        var isEmptySequence = machine.ElementCount == 0;
        machine.PopState();

        // Empty sequence
        if (isEmptySequence)
        {
            WriteEmptyFlowSequence();
            var lineBreak = machine.Current.State is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            if (lineBreak)
            {
                WriteNewLine();
            }
        }

        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                if (!isEmptySequence)
                {
                    machine.IndentationManager.DecreaseIndent();
                }
                machine.ElementCount++;
                break;

            case EmitState.BlockMappingKey:
                throw new YamlException("Complex key is not supported.");

            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                machine.ElementCount++;
                break;

            case EmitState.FlowSequenceEntry:
                machine.ElementCount++;
                break;
        }
    }
}
