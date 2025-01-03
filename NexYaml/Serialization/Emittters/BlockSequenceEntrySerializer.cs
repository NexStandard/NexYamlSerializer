using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockSequenceEntrySerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockSequenceEntry;

    public void Begin()
    {
        switch (emitter.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                emitter.WriteBlockSequenceEntryHeader();
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

        emitter.Next = emitter.Map(State);
    }

    public void WriteScalar(ReadOnlySpan<byte> output)
    {
        // first nested element
        if (emitter.IsFirstElement())
        {
            switch (emitter.Previous.State)
            {
                case EmitState.BlockSequenceEntry:
                    emitter.IndentationManager.IncreaseIndent();
                    emitter.WriteNewLine();
                    break;
                case EmitState.BlockMappingValue:
                    emitter.WriteNewLine();
                    break;
            }
        }

        emitter.WriteIndent()
            .WriteSequenceSeparator();

        // Write tag
        if (emitter.TryGetTag(out var tag))
        {
            emitter.WriteRaw(tag);
            emitter.WriteNewLine();
            emitter.WriteIndent();
        }
        emitter.WriteRaw(output);
        emitter.WriteNewLine();

        emitter.ElementCount++;
    }

    public void End()
    {
        var isEmptySequence = emitter.ElementCount == 0;
        emitter.PopState();

        // Empty sequence
        if (isEmptySequence)
        {
            emitter.WriteEmptyFlowSequence();
            var lineBreak = emitter.Current.State is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            if (lineBreak)
            {
                emitter.WriteNewLine();
            }
        }

        switch (emitter.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                if (!isEmptySequence)
                {
                    emitter.IndentationManager.DecreaseIndent();
                }
                emitter.ElementCount++;
                break;

            case EmitState.BlockMappingKey:
                throw new YamlException("Complex key is not supported.");

            case EmitState.BlockMappingValue:
                emitter.Current = emitter.Map(EmitState.BlockMappingKey);
                emitter.ElementCount++;
                break;

            case EmitState.FlowSequenceEntry:
                emitter.ElementCount++;
                break;
        }
    }
}
