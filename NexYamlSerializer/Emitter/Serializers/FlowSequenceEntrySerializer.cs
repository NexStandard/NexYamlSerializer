using NexVYaml.Emitter;
using NexVYaml.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowSequenceEntrySerializer(Utf8YamlEmitter emitter) : ISerializer
{
    public EmitState State { get; }

    public void Begin()
    {
        switch (emitter.StateStack.Current)
        {
            case EmitState.BlockMappingKey:
                throw new YamlEmitterException("To start block-mapping in the mapping key is not supported.");
            case EmitState.FlowMappingKey:
                throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");

            case EmitState.BlockSequenceEntry:
                throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");
            case EmitState.FlowSequenceEntry:
                {
                    break;
                }
            case EmitState.BlockMappingValue:
                break;
            default:
                emitter.WriteRaw(YamlCodes.FlowSequenceStart);
                break;
        }
        emitter.PushState(EmitState.FlowSequenceEntry);
    }

    public void BeginScalar(Span<byte> output, ref int offset)
    {

        if (emitter.StateStack.Previous is not EmitState.BlockMappingValue)
            return;
        if (emitter.IsFirstElement)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                output[offset++] = YamlCodes.Space;
            }

            EmitCodes.FlowSequenceEntryHeader.CopyTo(output[offset..]);
            offset += EmitCodes.FlowSequenceEntryHeader.Length;
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
        }

        var suffixLength = 1;
        if (needsLineBreak) suffixLength++;

        var offset = 0;
        var output = emitter.Writer.GetSpan(suffixLength);
        output[offset++] = YamlCodes.FlowSequenceEnd;
        if (needsLineBreak)
        {
            output[offset++] = YamlCodes.Lf;
        }
        emitter.Writer.Advance(offset);
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
        emitter.currentElementCount++;
    }
}
