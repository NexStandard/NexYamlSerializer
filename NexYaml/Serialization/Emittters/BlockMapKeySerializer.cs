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
            case EmitState.BlockSequenceEntry:
            {
                WriteIndent();
                WriteSequenceIdentifier();
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

        machine.Next = machine.Map(State);
        // Write tag if applicable
        if (machine.TryGetTag(out var tag2))
        {
            WriteRaw(tag2);
            WriteNewLine();
            WriteIndent();
        }
    }

    public override void Begin(string tag)
    {

    }
    /// <summary>
    /// Writes the scalar value of the key to the YAML output. This method handles the writing of the key in a block mapping,
    /// applying the correct indentation, writing any applicable tags, and writing the key itself.
    /// </summary>
    /// <param name="output">The scalar key value to be written to the YAML output.</param>
    public override void WriteScalar(ReadOnlySpan<char> output)
    {
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
        machine.PopState();

        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;
            case EmitState.BlockMappingValue:
                machine.IndentationManager.DecreaseIndent();
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                break;
        }
    }
}