using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexYamlSerializer.Emitter.Serializers;
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

    public void WriteScalar(ReadOnlySpan<char> output)
    {
        // first nested element
        if (emitter.IsFirstElement)
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
        if (emitter.tagStack.TryPop(out var tag))
        {
            emitter.WriteRaw(tag)
                .WriteNewLine()
                .WriteIndent();
        }
        emitter.WriteRaw(output)
            .WriteNewLine();

        emitter.currentElementCount++;
    }

    public void End()
    {
        var isEmptySequence = emitter.currentElementCount == 0;
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
                emitter.currentElementCount++;
                break;

            case EmitState.BlockMappingKey:
                throw new YamlException("Complex key is not supported.");

            case EmitState.BlockMappingValue:
                emitter.Current = emitter.Map(EmitState.BlockMappingKey);
                emitter.currentElementCount++;
                break;

            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
        }
    }
}
