using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
/// <summary>
/// <para>
/// The <see cref="BlockMapKeySerializer"/> class is an implementation of the <see cref="IEmitter"/> abstract class.
/// It is responsible for handling the serialization of a block mapping key in the YAML document.
/// Specifically, it writes the appropriate indentation, tags, and values for keys in a block mapping structure,
/// taking into account whether the current state is part of a block sequence or a flow sequence.
/// </para>
/// <para>
/// This class ensures that the correct formatting is applied when serializing keys within block mappings,
/// including the handling of indentation, tag writing, and transitioning to the next state within the emitter's state machine.
/// </para>
/// </summary>
internal class BlockMapKeySerializer : IEmitter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlockMapKeySerializer"/> class.
    /// </summary>
    /// <param name="writer">The <see cref="YamlWriter"/> used to emit YAML content.</param>
    /// <param name="machine">The <see cref="EmitterStateMachine"/> that governs the YAML emission state.</param>
    public BlockMapKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    /// <summary>
    /// Gets the current emission state, which is <see cref="EmitState.BlockMappingKey"/> for this serializer.
    /// </summary>
    public override EmitState State { get; } = EmitState.BlockMappingKey;

    /// <summary>
    /// Begins the serialization of the block mapping key. This method determines the correct course of action 
    /// depending on the previous state in the state machine, ensuring that the appropriate format is applied.
    /// </summary>
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
        switch (machine.Previous.State)
        {
            case EmitState.BlockSequenceEntry:
            {
                machine.IndentationManager.IncreaseIndent();

                // Try writing the tag, if present
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
                break;
            }
            case EmitState.BlockMappingValue:
            {
                machine.IndentationManager.IncreaseIndent();
                // Try writing the tag, if present
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

        // Write tag if applicable
        if (machine.TryGetTag(out var tag2))
        {
            WriteRaw(tag2);
            WriteNewLine();
            WriteIndent();
        }
    }

    /// <summary>
    /// Writes the scalar value of the key to the YAML output. This method handles the writing of the key in a block mapping,
    /// applying the correct indentation, writing any applicable tags, and writing the key itself.
    /// </summary>
    /// <param name="output">The scalar key value to be written to the YAML output.</param>
    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        if (machine.IsFirstElement)
        {

        }
        else
        {
            WriteIndent();
        }
        WriteRaw(output);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.BlockMappingValue);
    }

    /// <summary>
    /// Ends the serialization of the block mapping key. Handles the transition of the state machine to the next state
    /// and applies necessary formatting for empty mappings and line breaks.
    /// </summary>
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
                // This case is not implemented, further clarification needed
                throw new NotImplementedException();
                break;

            case EmitState.FlowSequenceEntry:
                machine.ElementCount++;
                break;
        }
    }
}