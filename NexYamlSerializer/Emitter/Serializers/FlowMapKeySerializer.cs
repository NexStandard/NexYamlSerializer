using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexYamlSerializer.Emitter.Serializers;
internal class FlowMapKeySerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowMappingKey;

    public void Begin()
    {
        var current = emitter.Current.State;
        if (current is EmitState.BlockSequenceEntry)
        {
            emitter.WriteIndent();
            emitter.WriteRaw(EmitCodes.BlockSequenceEntryHeader);
            emitter.WriteRaw(EmitCodes.FlowMappingStart);
            emitter.WriteBlockSequenceEntryHeader();
        }
        else if (current is EmitState.FlowSequenceEntry)
        {
            if (emitter.currentElementCount > 0)
            {
                emitter.WriteRaw(EmitCodes.FlowSequenceSeparator);
            }
        }
        else if (current is EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
        }
        emitter.Next = emitter.Map(State);
    }

    public void BeginScalar(Span<byte> output)
    {
        if (emitter.IsFirstElement)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                emitter.WriteRaw(tag);
                emitter.WriteRaw(EmitCodes.Space);
                emitter.WriteIndent();
            }
            emitter.WriteRaw(EmitCodes.FlowMappingStart);
        }
        if (!emitter.IsFirstElement)
        {
            emitter.WriteRaw(EmitCodes.FlowSequenceSeparator);
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
            if (emitter.tagStack.TryPop(out var tag))
            {
                emitter.WriteRaw(tag);
                emitter.WriteRaw(" ");
                emitter.WriteRaw(EmitCodes.FlowMappingEmpty);
                emitter.WriteRaw(YamlCodes.Lf);
                writeFlowMapEnd = false;
            }
            else
            {
                emitter.WriteRaw(EmitCodes.FlowMappingEmpty);
                emitter.WriteRaw(YamlCodes.Lf);
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

        if(writeFlowMapEnd)
        {
            emitter.WriteRaw(EmitCodes.FlowMappingEnd);
        }
        
        if (needsLineBreak)
        {
            emitter.WriteRaw(YamlCodes.Lf);
        }
    }

    public void EndScalar()
    {
        emitter.WriteRaw(EmitCodes.MappingKeyFooter);
        emitter.Current = emitter.Map(EmitState.FlowMappingValue);
    }
}
