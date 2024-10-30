using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public void BeginScalar(Span<byte> output, ref int offset)
    {
        // first nested element
        if (emitter.IsFirstElement)
        {
            emitter.Writer.Advance(offset);
            switch (emitter.Previous.State)
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

    public void End()
    {
        var isEmptySequence = emitter.currentElementCount == 0;
        emitter.PopState();

        // Empty sequence
        if (isEmptySequence)
        {
            var lineBreak = emitter.Current.State is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            emitter.WriteRaw(EmitCodes.FlowSequenceEmpty, false, lineBreak);
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

    public void EndScalar(Span<byte> output, ref int offset)
    {
        output[offset++] = YamlCodes.Lf;
        emitter.currentElementCount++;
    }
}
