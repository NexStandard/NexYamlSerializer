using NexVYaml.Internal;
using NexYamlSerializer.Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml.Emitter;
public partial class Utf8YamlEmitter
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginScalar(Span<byte> output, ref int offset)
    {
        switch (StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                {
                    // first nested element
                    if (IsFirstElement)
                    {
                        var output2 = Writer.GetSpan(EmitCodes.FlowSequenceSeparator.Length + 1);
                        var offset2 = 0;
                        EmitCodes.FlowSequenceSeparator.CopyTo(output2);
                        offset2 += EmitCodes.FlowSequenceSeparator.Length;
                        output2[offset2++] = YamlCodes.FlowSequenceStart;
                        Writer.Advance(offset);
                        switch (StateStack.Previous)
                        {
                            case EmitState.BlockSequenceEntry:
                                IndentationManager.IncreaseIndent();
                                output[offset++] = YamlCodes.Lf;
                                break;
                            case EmitState.BlockMappingValue:
                                output[offset++] = YamlCodes.Lf;
                                break;
                        }
                    }
                    WriteIndent(output, ref offset);
                    EmitCodes.BlockSequenceEntryHeader.CopyTo(output[offset..]);
                    offset += EmitCodes.BlockSequenceEntryHeader.Length;

                    // Write tag
                    if (tagStack.TryPop(out var tag))
                    {
                        offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                        output[offset++] = YamlCodes.Lf;
                        WriteIndent(output, ref offset);
                    }
                    break;
                }
            case EmitState.FlowSequenceEntry:
                if (StateStack.Previous is not EmitState.BlockMappingValue)
                    break;
                if(IsFirstElement)
                {
                    if (tagStack.TryPop(out var tag))
                    {
                        offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                        output[offset++] = YamlCodes.Space;
                    }

                    EmitCodes.FlowSequenceEntryHeader.CopyTo(output[offset..]);
                    offset += EmitCodes.FlowSequenceEntryHeader.Length;
                }
                else
                {
                    EmitCodes.FlowSequenceSeparator.CopyTo(output[offset..]);
                    offset += EmitCodes.FlowSequenceSeparator.Length;
                    break;
                }
                break;
            case EmitState.BlockMappingKey:
                blockMapKeySerializer.BeginScalar(output,ref offset);
                break;
            case EmitState.BlockMappingValue:
                break;
            case EmitState.FlowMappingValue: break;
            case EmitState.FlowMappingKey:
                flowMapKeySerializer.BeginScalar(output,ref offset);
                break;
            case EmitState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(StateStack.Current.ToString());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndScalar(Span<byte> output, ref int offset)
    {
        switch (StateStack.Current)
        {
            case EmitState.BlockSequenceEntry:
                output[offset++] = YamlCodes.Lf;
                currentElementCount++;
                break;
            case EmitState.BlockMappingKey:
                blockMapKeySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.FlowMappingKey:
                flowMapKeySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.FlowMappingValue:
                StateStack.Current = EmitState.FlowMappingKey;
                currentElementCount++;
                break;
            case EmitState.BlockMappingValue:
                output[offset++] = YamlCodes.Lf;
                StateStack.Current = EmitState.BlockMappingKey;
                currentElementCount++;
                break;
            case EmitState.FlowSequenceEntry:
                currentElementCount++;
                break;
            case EmitState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(StateStack.Current.ToString());
        }
        Writer.Advance(offset);
    }
}
