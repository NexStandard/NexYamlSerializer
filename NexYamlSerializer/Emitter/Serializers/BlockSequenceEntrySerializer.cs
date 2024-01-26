using NexVYaml.Emitter;
using NexVYaml.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockSequenceEntrySerializer(Utf8YamlEmitter emitter) : ISerializer
{
    public EmitState State { get; }

    public void Begin()
    {
        switch (emitter.StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                emitter.WriteBlockSequenceEntryHeader();
                break;

            case EmitState.FlowSequenceEntry:
                throw new YamlEmitterException(
                    "To start block-sequence in the flow-sequence is not supported.");
            case EmitState.FlowMappingKey:
                throw new YamlEmitterException(
                    "To start block-sequence in the flow mapping key is not supported.");
            case EmitState.BlockMappingKey:
                throw new YamlEmitterException(
                    "To start block-sequence in the mapping key is not supported.");
        }

        emitter.PushState(EmitState.BlockSequenceEntry);
    }

    public void BeginScalar(Span<byte> output, ref int offset)
    {
        // first nested element
        if (emitter.IsFirstElement)
        {
            var output2 = emitter.Writer.GetSpan(EmitCodes.FlowSequenceSeparator.Length + 1);
            var offset2 = 0;
            EmitCodes.FlowSequenceSeparator.CopyTo(output2);
            offset2 += EmitCodes.FlowSequenceSeparator.Length;
            output2[offset2++] = YamlCodes.FlowSequenceStart;
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

    public void End()
    {
        throw new NotImplementedException();
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
        output[offset++] = YamlCodes.Lf;
        emitter.currentElementCount++;
    }
}
