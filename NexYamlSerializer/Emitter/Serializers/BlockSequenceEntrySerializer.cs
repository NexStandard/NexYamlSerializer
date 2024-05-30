using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockSequenceEntrySerializer(Utf8YamlEmitter emitter) : EmitterSerializer
{
    public override EmitState State { get; } = EmitState.BlockSequenceEntry;

    public override void Begin()
    {
        switch (emitter.StateStack.Current)
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

        emitter.PushState(State);
    }

    public override void BeginScalar(Span<byte> output, ref int offset)
    {
        // first nested element
        if (emitter.IsFirstElement)
        {
            var output2 = emitter.Writer.GetSpan(EmitCodes.FlowSequenceSeparator.Length + 1);
            var offset2 = 0;
            EmitCodes.FlowSequenceSeparator.CopyTo(output2);
            offset2 += EmitCodes.FlowSequenceSeparator.Length;
            output2[offset2 + 1] = YamlCodes.FlowSequenceStart;
            emitter.Writer.Advance(offset);
            switch (emitter.StateStack.Previous)
            {
                case EmitState.BlockSequenceEntry:
                    emitter.IndentationManager.IncreaseIndent();
                    output[offset++] = YamlCodes.Lf;
                    break;
                case EmitState.BlockMappingValue:
                    output[offset++] = YamlCodes.Lf;
                    break;
            }
        }
        emitter.WriteIndent(output, ref offset);
        EmitCodes.BlockSequenceEntryHeader.CopyTo(output[offset..]);
        offset += EmitCodes.BlockSequenceEntryHeader.Length;

        // Write tag
        if (emitter.tagStack.TryPop(out var tag))
        {
            offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
            output[offset++] = YamlCodes.Lf;
            emitter.WriteIndent(output, ref offset);
        }
    }

    public override void End()
    {
        var isEmptySequence = emitter.currentElementCount == 0;
        emitter.PopState();

        // Empty sequence
        if (isEmptySequence)
        {
            var lineBreak = emitter.StateStack.Current is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            emitter.WriteRaw(EmitCodes.FlowSequenceEmpty, false, lineBreak);
        }

        switch (emitter.StateStack.Current)
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
                emitter.StateStack.Current = EmitState.BlockMappingKey;
                emitter.currentElementCount++;
                break;

            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
        }
    }

    public override void EndScalar(Span<byte> output, ref int offset)
    {
        output[offset++] = YamlCodes.Lf;
        emitter.currentElementCount++;
    }
}
