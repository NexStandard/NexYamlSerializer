using NexVYaml.Emitter;
using NexVYaml.Internal;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockMapKeySerializer(Utf8YamlEmitter emitter) : ISerializer
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
            case EmitState.FlowSequenceEntry:
                throw new YamlEmitterException("Cannot start block-mapping in the flow-sequence");

            case EmitState.BlockSequenceEntry:
                {
                    emitter.WriteBlockSequenceEntryHeader();
                    break;
                }
        }
        emitter.PushState(EmitState.BlockMappingKey);

    }
    public void BeginScalar(Span<byte> output, ref int offset)
    {
        if (emitter.IsFirstElement)
        {
            switch (emitter.StateStack.Previous)
            {
                case EmitState.BlockSequenceEntry:
                    {
                        emitter.IndentationManager.IncreaseIndent();

                        // Try write tag
                        if (emitter.tagStack.TryPop(out var tag))
                        {
                            offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                            output[offset++] = YamlCodes.Lf;
                            emitter.WriteIndent(output, ref offset);
                        }
                        else
                        {
                            emitter.WriteIndent(output, ref offset, emitter.Options.IndentWidth - 2);
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
                            offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                        }
                        output[offset++] = YamlCodes.Lf;
                        emitter.WriteIndent(output, ref offset);
                        break;
                    }
                default:
                    emitter.WriteIndent(output, ref offset);
                    break;
            }

            // Write tag
            if (emitter.tagStack.TryPop(out var tag2))
            {
                offset += StringEncoding.Utf8.GetBytes(tag2, output[offset..]);
                output[offset++] = YamlCodes.Lf;
                emitter.WriteIndent(output, ref offset);
            }
        }
        else
        {
            emitter.WriteIndent(output, ref offset);
        }
    }
    public void EndScalar(Span<byte> output, ref int offset)
    {
        output[offset++] = YamlCodes.Lf;
        emitter.StateStack.Current = EmitState.BlockMappingKey;
        emitter.currentElementCount++;
        emitter.Writer.Advance(offset);
    }
    public void BeginBlockSequence()
    {

        var current = emitter.StateStack.Current;
        if (current is EmitState.BlockSequenceEntry)
        {
            emitter.WriteBlockSequenceEntryHeader();
        }
        if (current is EmitState.FlowSequenceEntry or EmitState.FlowMappingKey or EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start block-sequence in the {current} is not supported");
        }

        emitter.PushState(EmitState.BlockSequenceEntry);
    }
    public void End()
    {
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

        if (isEmptyMapping)
        {
            var lineBreak = emitter.StateStack.Current is EmitState.BlockSequenceEntry or EmitState.BlockMappingValue;
            if (emitter.tagStack.TryPop(out var tag))
            {
                var tagBytes = StringEncoding.Utf8.GetBytes(tag + " "); // TODO:
                emitter.WriteRaw(tagBytes, EmitCodes.FlowMappingEmpty, false, lineBreak);
            }
            else
            {
                emitter.WriteRaw(EmitCodes.FlowMappingEmpty, false, false);
            }
        }

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
}
