using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapKeySerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowMappingKey;

    public void Begin()
    {
        var current = emitter.Current.State;
        if (current is EmitState.BlockSequenceEntry)
        {
            var output = emitter.Writer.GetSpan((emitter.CurrentIndentLevel * UTF8Stream.IndentWidth) + EmitCodes.BlockSequenceEntryHeader.Length + EmitCodes.FlowMappingStart.Length);
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
        emitter.Next = emitter.Map(State);
    }

    public void BeginScalar(Span<byte> output, ref int offset)
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

    public void End()
    {
        if (emitter.Current.State is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
        {
            throw new YamlException($"Invalid block mapping end: {emitter.StateStack.Current}");
        }
        var isEmptyMapping = emitter.currentElementCount <= 0;
        bool writeFlowMapEnd = true;
        if (isEmptyMapping)
        {
            var lineBreak = emitter.Current.State is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            if (emitter.tagStack.TryPop(out var tag))
            {
                var tagBytes = StringEncoding.Utf8.GetBytes(tag + " "); // TODO:
                emitter.WriteRaw(tagBytes, EmitCodes.FlowMappingEmpty, false, lineBreak);
                writeFlowMapEnd = false;
            }
            else
            {
                emitter.WriteRaw(EmitCodes.FlowMappingEmpty, false, lineBreak);
                writeFlowMapEnd = false;
            }
        }
        emitter.PopState();

        var needsLineBreak = false;
        switch (emitter.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                emitter.currentElementCount++;
                break;
            case EmitState.BlockMappingValue:
                emitter.Current = emitter.Map(EmitState.BlockMappingKey);
                needsLineBreak = true;
                emitter.currentElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
            case EmitState.FlowMappingValue:
                emitter.Current = emitter.Map(EmitState.FlowMappingKey);
                emitter.currentElementCount++;
                break;
        }
        var suffixLength = 0;
        if(writeFlowMapEnd)
        {
            suffixLength = EmitCodes.FlowMappingEnd.Length;
        }

        if (needsLineBreak) 
            suffixLength++;

        var offset = 0;
        var output = emitter.Writer.GetSpan(suffixLength);
        if(writeFlowMapEnd)
        {
            EmitCodes.FlowMappingEnd.CopyTo(output[offset..]);
            offset += EmitCodes.FlowMappingEnd.Length;
        }

        if (needsLineBreak)
        {
            output[offset++] = YamlCodes.Lf;
        }
        emitter.Writer.Advance(offset);
    }

    public void EndScalar()
    {
        emitter.WriteRaw(EmitCodes.MappingKeyFooter);
        emitter.Current = emitter.Map(EmitState.FlowMappingValue);
    }
}
