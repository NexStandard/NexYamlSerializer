using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
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
                if (!emitter.IsFirstElement())
                {
                    emitter.WriteFlowSequenceSeparator();
                }
                emitter.WriteFlowSequenceStart();
                break;
            }
            case EmitState.BlockMappingValue:
            {
                if (!emitter.IsFirstElement())
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
        if (emitter.IsFirstElement())
        {
            if (emitter.TryGetTag(out var tag))
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
        emitter.ElementCount++;
    }

    public void End()
    {
        emitter.PopState();

        var needsLineBreak = false;
        switch (emitter.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                emitter.ElementCount++;
                break;
            case EmitState.BlockMappingValue:
                emitter.Current = emitter.Map(EmitState.BlockMappingKey);
                needsLineBreak = true;
                emitter.ElementCount++;
                break;
            case EmitState.FlowMappingValue:
                emitter.Current = emitter.Map(EmitState.FlowMappingKey);
                emitter.ElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                emitter.ElementCount++;
                break;
        }

        emitter.WriteFlowSequenceEnd();
        if (needsLineBreak)
        {
            emitter.WriteNewLine();
        }
    }
}
