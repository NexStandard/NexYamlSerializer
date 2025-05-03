using System.Diagnostics;
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
    public override EmitState State => EmitState.BlockMappingKey;

    /// <summary>
    /// Begins the serialization of the block mapping key. This method determines the correct course of action 
    /// depending on the previous state in the state machine, ensuring that the appropriate format is applied.
    /// </summary>
    public override EmitResult Begin(BeginContext context)
    {
        switch (context.Emitter.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteIndent();
                WriteSequenceIdentifier();
                context.Indentation.IncreaseIndent();

                // Try writing the tag, if present
                if (context.NeedsTag)
                {
                    WriteRaw(context.Tag);
                    WriteNewLine();
                    WriteIndent();
                }
                else
                {
                    WriteIndent(context.Indentation.IndentWidth - 2);
                }
                break;
            case EmitState.BlockMappingValue:

                context.Indentation.IncreaseIndent();
                if (context.NeedsTag)
                {
                    WriteRaw(context.Tag);
                }
                WriteNewLine();
                WriteIndent();

                break;
            case EmitState.None:
                if (context.NeedsTag)
                {
                    WriteRaw(context.Tag);
                    WriteNewLine();
                    WriteIndent();
                }
                break;
            default:
                throw new YamlException($"Unexpected state {machine.Current.State} for next state {State}");
        }
        return new EmitResult(this);
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.BlockMappingValue);
    }

    public override void End()
    {
        machine.PopState();

        if (machine.Current.State != EmitState.None)
        {
            throw new YamlException($"Unexpected end state for {State}: {machine.Current.State}");
        }
    }
}