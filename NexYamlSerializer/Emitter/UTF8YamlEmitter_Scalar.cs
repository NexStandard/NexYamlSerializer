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
    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        var offset = 0;
        var output = Writer.GetSpan(CalculateMaxScalarBufferLength(value.Length));

        BeginScalar(output, ref offset);
        value.CopyTo(output[offset..]);
        offset += value.Length;
        EndScalar(output, ref offset);
    }
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
                    BlockSequenceEntryHeader.CopyTo(output[offset..]);
                    offset += BlockSequenceEntryHeader.Length;

                    // Write tag
                    if (tagStack.TryPop(out var tag))
                    {
                        offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                        output[offset++] = YamlCodes.Lf;
                        WriteIndent(output, ref offset);
                    }
                    break;
                }
            case EmitState.BlockMappingKey:
                {
                    if (IsFirstElement)
                    {
                        switch (StateStack.Previous)
                        {
                            case EmitState.BlockSequenceEntry:
                                {
                                    IndentationManager.IncreaseIndent();

                                    // Try write tag
                                    if (tagStack.TryPop(out var tag))
                                    {
                                        offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                                        output[offset++] = YamlCodes.Lf;
                                        WriteIndent(output, ref offset);
                                    }
                                    else
                                    {
                                        WriteIndent(output, ref offset, Options.IndentWidth - 2);
                                    }
                                    // The first key in block-sequence is like so that: "- key: .."
                                    break;
                                }
                            case EmitState.BlockMappingValue:
                                {
                                    IndentationManager.IncreaseIndent();
                                    // Try write tag
                                    if (tagStack.TryPop(out var tag))
                                    {
                                        offset += StringEncoding.Utf8.GetBytes(tag, output[offset..]);
                                    }
                                    output[offset++] = YamlCodes.Lf;
                                    WriteIndent(output, ref offset);
                                    break;
                                }
                            default:
                                WriteIndent(output, ref offset);
                                break;
                        }

                        // Write tag
                        if (tagStack.TryPop(out var tag2))
                        {
                            offset += StringEncoding.Utf8.GetBytes(tag2, output[offset..]);
                            output[offset++] = YamlCodes.Lf;
                            WriteIndent(output, ref offset);
                        }
                    }
                    else
                    {
                        WriteIndent(output, ref offset);
                    }
                    break;
                }
            case EmitState.BlockMappingValue:
                break;

            case EmitState.FlowMappingKey:
                FlowMappingStart.CopyTo(output[offset..]);
                offset += FlowMappingStart.Length;
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
            case EmitState.BlockMappingKey or EmitState.FlowMappingKey:
                MappingKeyFooter.CopyTo(output[offset..]);
                offset += MappingKeyFooter.Length;
                StateStack.Current = EmitState.BlockMappingValue;
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
