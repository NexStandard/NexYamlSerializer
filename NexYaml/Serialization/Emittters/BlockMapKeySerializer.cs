using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapKeySerializer : IEmitter
{
    public BlockMapKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.BlockMappingKey;

    public override void Begin()
    {
        switch (machine.Current.State)
        {
            case EmitState.BlockMappingKey:
                throw new YamlException("To start block-mapping in the mapping key is not supported.");
            case EmitState.FlowMappingKey:
                throw new YamlException("To start flow-mapping in the mapping key is not supported.");
            case EmitState.FlowSequenceEntry:
                throw new YamlException("Cannot start block-mapping in the flow-sequence");

            case EmitState.BlockSequenceEntry:
            {
                WriteBlockSequenceEntryHeader();
                break;
            }
        }
        machine.Next = machine.Map(State);
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        if (machine.IsFirstElement)
        {
            switch (machine.Previous.State)
            {
                case EmitState.BlockSequenceEntry:
                {
                    machine.IndentationManager.IncreaseIndent();

                    // Try write tag
                    if (machine.TryGetTag(out var tag))
                    {
                        WriteRaw(tag);
                        WriteRaw(YamlCodes.NewLine);
                        WriteIndent();
                    }
                    else
                    {
                        WriteIndent(machine.IndentationManager.IndentWidth - 2);
                    }
                    // The first key in block-sequence is like so that: "- key: .."
                    break;
                }
                case EmitState.BlockMappingValue:
                {
                    machine.IndentationManager.IncreaseIndent();
                    // Try write tag
                    if (machine.TryGetTag(out var tag))
                    {
                        WriteRaw(tag);
                    }
                    WriteNewLine();
                    WriteIndent();
                    break;
                }
                default:
                    WriteIndent();
                    break;
            }

            // Write tag
            if (machine.TryGetTag(out var tag2))
            {
                WriteRaw(tag2);
                WriteNewLine();
                WriteIndent();
            }
        }
        else
        {
            WriteIndent();
        }
        WriteRaw(output);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.BlockMappingValue);
    }

    public override void End()
    {
        var isEmptyMapping = machine.IsFirstElement;
        machine.PopState();

        if (isEmptyMapping)
        {
            var lineBreak = machine.Current.State is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            if (machine.TryGetTag(out var tag))
            {
                WriteRaw(tag);
                WriteSpace();
            }
            WriteEmptyFlowMapping();
            if (lineBreak)
            {
                WriteNewLine();
            }
        }

        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                if (!isEmptyMapping)
                {
                    machine.IndentationManager.DecreaseIndent();
                }
                machine.ElementCount++;
                break;

            case EmitState.BlockMappingValue:
                if (!isEmptyMapping)
                {
                    machine.IndentationManager.DecreaseIndent();
                }
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                machine.ElementCount++;
                break;
            case EmitState.FlowMappingValue:
                // TODO: What should be here?
                /*
                if (!isEmptyMapping)
                {
                    emitter.IndentationManager.DecreaseIndent();
                }
                emitter.StateStack.Current = EmitState.BlockMappingKey;
                emitter.currentElementCount++;
                */
                throw new NotImplementedException();
                break;
            case EmitState.FlowSequenceEntry:
                machine.ElementCount++;
                break;
        }
    }
}
