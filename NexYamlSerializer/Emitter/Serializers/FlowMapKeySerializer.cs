using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapKeySerializer(Utf8YamlEmitter emitter) : EmitterSerializer
{
    public override EmitState State { get; } = EmitState.FlowMappingKey;

    public override void Begin()
    {
        var current = emitter.StateStack.Current;
        if (current is EmitState.BlockSequenceEntry)
        {
            var output = emitter.Writer.GetSpan((emitter.CurrentIndentLevel * emitter.Options.IndentWidth) + EmitCodes.BlockSequenceEntryHeader.Length + EmitCodes.FlowMappingStart.Length);
            var offset = 0;
            emitter.WriteIndent(output, ref offset);

            EmitCodes.BlockSequenceEntryHeader.CopyTo(output[offset..]);
            offset += EmitCodes.BlockSequenceEntryHeader.Length;

            EmitCodes.FlowMappingStart.CopyTo(output[offset..]);
            offset += EmitCodes.FlowMappingStart.Length;
            emitter.Writer.Advance(offset);
            emitter.WriteBlockSequenceEntryHeader();
        }
        else if (current is EmitState.FlowSequenceEntry)
        {
            var output = emitter.Writer.GetSpan(EmitCodes.FlowSequenceSeparator.Length + EmitCodes.FlowMappingStart.Length);
            var offset = 0;
            if (emitter.currentElementCount > 0)
            {
                EmitCodes.FlowSequenceSeparator.CopyTo(output);
                offset += EmitCodes.FlowSequenceSeparator.Length;
            }
            emitter.Writer.Advance(offset);
        }
        else if (current is EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
        }
        emitter.PushState(State);
    }

    public override void BeginScalar(Span<byte> output, ref int offset)
    {
        if (emitter.IsFirstElement)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                output[offset++] = YamlCodes.Space;
                emitter.WriteIndent(output, ref offset);
            }

            EmitCodes.FlowMappingStart.CopyTo(output[offset..]);
            offset += EmitCodes.FlowMappingStart.Length;
        }
        if (!emitter.IsFirstElement)
        {
            EmitCodes.FlowSequenceSeparator.CopyTo(output[offset..]);
            offset += EmitCodes.FlowSequenceSeparator.Length;
        }
    }

    public override void End()
    {
        if (emitter.StateStack.Current is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
        {
            throw new YamlEmitterException($"Invalid block mapping end: {emitter.StateStack.Current}");
        }

        emitter.PopState();

        var needsLineBreak = false;
        switch (emitter.StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                emitter.currentElementCount++;
                break;
            case EmitState.BlockMappingValue:
                emitter.StateStack.Current = EmitState.BlockMappingKey; // end mapping value
                needsLineBreak = true;
                emitter.currentElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
            case EmitState.FlowMappingValue:
                emitter.StateStack.Current = EmitState.FlowMappingKey;
                emitter.currentElementCount++;
                break;
        }

        var suffixLength = EmitCodes.FlowMappingEnd.Length;
        if (needsLineBreak) 
            suffixLength++;

        var offset = 0;
        var output = emitter.Writer.GetSpan(suffixLength);
        EmitCodes.FlowMappingEnd.CopyTo(output[offset..]);
        offset += EmitCodes.FlowMappingEnd.Length;
        if (needsLineBreak)
        {
            output[offset++] = YamlCodes.Lf;
        }
        emitter.Writer.Advance(offset);
    }

    public override void EndScalar(Span<byte> output, ref int offset)
    {
        EmitCodes.MappingKeyFooter.CopyTo(output[offset..]);
        offset += EmitCodes.MappingKeyFooter.Length;
        emitter.StateStack.Current = EmitState.FlowMappingValue;
    }
}
