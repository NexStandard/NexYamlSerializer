using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    emitter.WriteRaw(EmitCodes.BlockSequenceEntryHeader);
                    emitter.WriteRaw(YamlCodes.FlowSequenceStart);
                    break;
                }
            case EmitState.FlowSequenceEntry:
                {
                    if (emitter.currentElementCount > 0)
                    {
                        emitter.WriteRaw(EmitCodes.FlowSequenceSeparator);
                    }
                    emitter.WriteRaw(YamlCodes.FlowSequenceStart);
                    break;
                }
            case EmitState.BlockMappingValue:
                {
                    if (emitter.currentElementCount > 0)
                    {
                        emitter.WriteRaw(EmitCodes.FlowSequenceSeparator);
                    }
                    emitter.WriteRaw(YamlCodes.FlowSequenceStart);
                }
                break;
            default:
                emitter.WriteRaw(YamlCodes.FlowSequenceStart);
                break;
        }
        emitter.Next = emitter.Map(State);
    }

    public void BeginScalar(Span<byte> output)
    {
        if (emitter.IsFirstElement)
        {
            if (emitter.tagStack.TryPop(out var tag))
            {
                StringEncoding.Utf8.GetBytes(tag.AsSpan(), emitter.Writer);
                emitter.Writer.Write([ YamlCodes.Space ]);
            }
        }
        else
        {
            emitter.Writer.Write(EmitCodes.FlowSequenceSeparator);
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

        emitter.WriteRaw(YamlCodes.FlowSequenceEnd);
        if (needsLineBreak)
        {
            emitter.WriteRaw(YamlCodes.Lf);
        }
    }

    public void EndScalar()
    {
        emitter.currentElementCount++;
    }
}
