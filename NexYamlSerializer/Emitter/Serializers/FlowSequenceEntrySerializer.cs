using NexVYaml;
using NexVYaml.Emitter;
using System;

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
                emitter.WriteIndent();
                emitter.WriteSequenceSeparator();
                emitter.WriteFlowMappingStart();
                break;
            }
            case EmitState.FlowSequenceEntry:
            {
                if (emitter.currentElementCount > 0)
                {
                    emitter.WriteFlowSequenceSeparator();
                }
                emitter.WriteFlowSequenceStart();
                break;
            }
            case EmitState.BlockMappingValue:
            {
                if (emitter.currentElementCount > 0)
                {
                    emitter.WriteFlowSequenceSeparator();
                }
                emitter.WriteFlowSequenceStart();
            }
            break;
            default:
                emitter.WriteFlowSequenceStart();
                break;
        }
        emitter.Next = emitter.Map(State);
    }

    public void WriteScalar(ReadOnlySpan<char> value)
    {
        if (emitter.IsFirstElement)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                emitter.WriteRaw(tag);
                emitter.WriteSpace();
            }
        }
        else
        {
            emitter.WriteFlowSequenceSeparator();
        }
        emitter.WriteRaw(value);
        emitter.currentElementCount++;
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

        emitter.WriteFlowSequenceEnd();
        if (needsLineBreak)
        {
            emitter.WriteNewLine();
        }
    }
}
