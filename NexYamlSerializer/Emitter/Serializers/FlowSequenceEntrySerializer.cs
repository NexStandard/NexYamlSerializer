using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowSequenceEntrySerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowSequenceEntry;

    public void Begin()
    {
        switch (emitter.Current.State)
        {
            case EmitState.BlockMappingKey:
                throw new YamlException("To start block-mapping in the mapping key is not supported.");
            case EmitState.BlockSequenceEntry:
                {
                    var output = emitter.Writer.GetSpan((emitter.CurrentIndentLevel * UTF8Stream.IndentWidth) + EmitCodes.BlockSequenceEntryHeader.Length + 1);
                    var offset = 0;
                    emitter.WriteIndent(output, ref offset);
                    EmitCodes.BlockSequenceEntryHeader.CopyTo(output[offset..]);
                    offset += EmitCodes.BlockSequenceEntryHeader.Length;
                    output[offset++] = YamlCodes.FlowSequenceStart;
                    emitter.Writer.Advance(offset);
                    break;
                }
            case EmitState.FlowSequenceEntry:
                {
                    var output = emitter.Writer.GetSpan(EmitCodes.FlowSequenceSeparator.Length + 1);
                    var offset = 0;
                    if (emitter.currentElementCount > 0)
                    {
                        EmitCodes.FlowSequenceSeparator.CopyTo(output);
                        offset += EmitCodes.FlowSequenceSeparator.Length;
                    }
                    output[offset++] = YamlCodes.FlowSequenceStart;
                    emitter.Writer.Advance(offset);
                    break;
                }
            case EmitState.BlockMappingValue:
                {
                    var output = emitter.Writer.GetSpan(EmitCodes.FlowSequenceSeparator.Length + 1);
                    var offset = 0;
                    if (emitter.currentElementCount > 0)
                    {
                        EmitCodes.FlowSequenceSeparator.CopyTo(output);
                        offset += EmitCodes.FlowSequenceSeparator.Length;
                    }
                    output[offset++] = YamlCodes.FlowSequenceStart;
                    emitter.Writer.Advance(offset);
                }
                break;
            default:
                emitter.WriteRaw(YamlCodes.FlowSequenceStart);
                break;
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
            }
        }
        else
        {
            EmitCodes.FlowSequenceSeparator.CopyTo(output[offset..]);
            offset += EmitCodes.FlowSequenceSeparator.Length;
        }
    }

    public void End()
    {
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
            case EmitState.FlowMappingValue:
                emitter.Current = emitter.Map(EmitState.FlowMappingKey);
                emitter.currentElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
        }

        var suffixLength = 1;
        if (needsLineBreak) 
            suffixLength++;

        var offset = 0;
        var output = emitter.Writer.GetSpan(suffixLength);
        output[offset++] = YamlCodes.FlowSequenceEnd;
        if (needsLineBreak)
        {
            output[offset++] = YamlCodes.Lf;
        }
        emitter.Writer.Advance(offset);
    }

    public void EndScalar()
    {
        emitter.currentElementCount++;
    }
}
