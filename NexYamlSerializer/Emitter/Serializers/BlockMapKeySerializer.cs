﻿using NexVYaml;
using NexVYaml.Emitter;
using NexYaml.Core;
using System;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockMapKeySerializer(Utf8YamlEmitter emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockMappingKey;

    public void Begin()
    {
        switch (emitter.StateStack.Current)
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
        emitter.PushState(State);

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
                            emitter.WriteIndent(output, ref offset, Utf8YamlEmitter.IndentWidth - 2);
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
         EmitCodes.MappingKeyFooter.CopyTo(output[offset..]);
         offset += EmitCodes.MappingKeyFooter.Length;
        emitter.StateStack.Current = EmitState.BlockMappingValue;
    }

    public void End()
    {
        var isEmptyMapping = emitter.currentElementCount <= 0;
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
                emitter.WriteRaw(EmitCodes.FlowMappingEmpty, false, lineBreak);
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
