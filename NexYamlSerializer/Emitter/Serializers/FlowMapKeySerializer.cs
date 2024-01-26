using NexVYaml.Emitter;
using NexVYaml.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapKeySerializer(Utf8YamlEmitter emitter) : ISerializer
{
    public EmitState State { get; } = EmitState.FlowMappingKey;

    public void Begin()
    {
        var current = emitter.StateStack.Current;
        if (current is EmitState.BlockSequenceEntry)
        {
            emitter.WriteBlockSequenceEntryHeader();
        }
        if (current is EmitState.BlockMappingKey or EmitState.BlockSequenceEntry or EmitState.FlowSequenceEntry)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
        }
        emitter.PushState(State);
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
            offset += 2;
        }
        if (!emitter.IsFirstElement)
        {
            EmitCodes.FlowSequenceSeparator.CopyTo(output[offset..]);
            offset += EmitCodes.FlowSequenceSeparator.Length;
        }
    }

    public void End()
    {
        if (emitter.StateStack.Current is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
        {
            throw new YamlEmitterException($"Invalid block mapping end: {emitter.StateStack.Current}");
        }
        var isEmptyMapping = emitter.currentElementCount <= 0;
        if (emitter.StateStack.Current == EmitState.FlowMappingKey && !isEmptyMapping)
        {
            emitter.WriteRaw(EmitCodes.FlowMappingEnd, false, true);
        }
        else if (emitter.StateStack.Current == EmitState.FlowMappingKey && isEmptyMapping)
        {
            emitter.WriteRaw(YamlCodes.FlowMapEnd);
        }
        emitter.PopState();
        switch (emitter.StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                if (!isEmptyMapping)
                {
                    emitter.IndentationManager.DecreaseIndent();
                }
                emitter.currentElementCount++;
                break;

            case EmitState.BlockMappingValue:
                if (!isEmptyMapping)
                {
                    emitter.IndentationManager.DecreaseIndent();
                }
                emitter.StateStack.Current = EmitState.BlockMappingKey;
                emitter.currentElementCount++;
                break;
            case EmitState.FlowMappingValue:
                if (!isEmptyMapping)
                {
                    emitter.IndentationManager.DecreaseIndent();
                }
                emitter.StateStack.Current = EmitState.BlockMappingKey;
                emitter.currentElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
        }
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
        EmitCodes.MappingKeyFooter.CopyTo(output[offset..]);
        offset += EmitCodes.MappingKeyFooter.Length;
        emitter.StateStack.Current = EmitState.FlowMappingValue;
    }
}
