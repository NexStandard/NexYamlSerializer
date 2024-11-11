using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapKeySerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockMappingKey;

    public void Begin()
    {
        switch (emitter.Current.State)
        {
            case EmitState.BlockMappingKey:
                throw new YamlException("To start block-mapping in the mapping key is not supported.");
            case EmitState.FlowMappingKey:
                throw new YamlException("To start flow-mapping in the mapping key is not supported.");
            case EmitState.FlowSequenceEntry:
                throw new YamlException("Cannot start block-mapping in the flow-sequence");

            case EmitState.BlockSequenceEntry:
                {
                    emitter.WriteBlockSequenceEntryHeader();
                    break;
                }
        }
        emitter.Next = emitter.Map(State);

    }
    public void WriteScalar(ReadOnlySpan<char> output)
    {
        if (emitter.IsFirstElement)
        {
            switch (emitter.Previous.State)
            {
                case EmitState.BlockSequenceEntry:
                    {
                        emitter.IndentationManager.IncreaseIndent();

                        // Try write tag
                        if (emitter.tagStack.TryPop(out var tag))
                        {
                            emitter.WriteRaw(tag);
                            emitter.WriteNewLine();
                            emitter.WriteIndent();
                        }
                        else
                        {
                            emitter.WriteIndent(emitter.IndentWidth - 2);
                        }
                        // The first key in block-sequence is like so that: "- key: .."
                        break;
                    }
                case EmitState.BlockMappingValue:
                    {
                        emitter.IndentationManager.IncreaseIndent();
                        // Try write tag
                        if (emitter.tagStack.TryPop(out var tag))
                        {
                            emitter.WriteRaw(tag);
                        }
                        emitter.WriteNewLine();
                        emitter.WriteIndent();
                        break;
                    }
                default:
                    emitter.WriteIndent();
                    break;
            }

            // Write tag
            if (emitter.tagStack.TryPop(out var tag2))
            {
                emitter.WriteRaw(tag2);
                emitter.WriteNewLine();
                emitter.WriteIndent();
            }
        }
        else
        {
            emitter.WriteIndent();
        }
        emitter.WriteRaw(output);
        emitter.WriteMappingKeyFooter();
        emitter.Current = emitter.Map(EmitState.BlockMappingValue);
    }

    public void End()
    {
        var isEmptyMapping = emitter.currentElementCount <= 0;
        emitter.PopState();

        if (isEmptyMapping)
        {
            var lineBreak = emitter.Current.State is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            if (emitter.tagStack.TryPop(out var tag))
            {
                emitter.WriteRaw(tag);
                emitter.WriteRaw(" ");
            }
            emitter.WriteEmptyFlowMapping();
            if (lineBreak)
            {
                emitter.WriteNewLine();
            }
        }

        switch (emitter.Current.State)
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
                emitter.Current = emitter.Map(EmitState.BlockMappingKey);
                emitter.currentElementCount++;
                break;
            case EmitState.FlowMappingValue:
                // TODO: What should be here?
                /*
                if (!isEmptyMapping)
                {
                    emitter.IndentationManager.DecreaseIndent();
                }
                emitter.StateStack.Current = EmitState.BlockMappingKey;
                emitter.currentElementCount++;
                */
                throw new NotImplementedException();
                break;
            case EmitState.FlowSequenceEntry:
                emitter.currentElementCount++;
                break;
        }
    }
}
