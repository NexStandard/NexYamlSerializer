using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapKeySerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.FlowMappingKey;

    public void Begin()
    {
        var current = emitter.Current.State;
        if (current is EmitState.BlockSequenceEntry)
        {
            emitter.WriteIndent()
                .WriteSequenceSeparator()
                .WriteFlowMappingStart();
            emitter.WriteBlockSequenceEntryHeader();
        }
        else if (current is EmitState.FlowSequenceEntry)
        {
            if (emitter.currentElementCount > 0)
            {
                emitter.WriteFlowSequenceSeparator();
            }
        }
        else if (current is EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
        }
        emitter.Next = emitter.Map(State);
    }

    public void WriteScalar(ReadOnlySpan<char> value)
    {
        if (emitter.IsFirstElement)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                emitter.WriteRaw(tag)
                    .WriteSpace()
                    .WriteIndent();
            }
            emitter.WriteFlowMappingStart();
        }
        if (!emitter.IsFirstElement)
        {
            emitter.WriteFlowSequenceSeparator();
        }
        emitter.WriteRaw(value);
        emitter.WriteMappingKeyFooter();
        emitter.Current = emitter.Map(EmitState.FlowMappingValue);
    }

    public void End()
    {
        if (emitter.Current.State is not EmitState.BlockMappingKey and not EmitState.FlowMappingKey)
        {
            throw new YamlException($"Invalid block mapping end: {emitter.StateStack.Current}");
        }
        var isEmptyMapping = emitter.currentElementCount <= 0;
        var writeFlowMapEnd = true;
        if (isEmptyMapping)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                emitter.WriteRaw(tag)
                    .WriteSpace()
                    .WriteEmptyFlowMapping()
                    .WriteNewLine();
                writeFlowMapEnd = false;
            }
            else
            {
                emitter.WriteEmptyFlowMapping()
                    .WriteNewLine();
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

        if (writeFlowMapEnd)
        {
            emitter.WriteFlowMappingEnd();
        }

        if (needsLineBreak)
        {
            emitter.WriteNewLine();
        }
    }
}
