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
                blockSequenceEntrySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.FlowSequenceEntry:
                flowSequenceEntrySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.BlockMappingKey:
                blockMapKeySerializer.BeginScalar(output, ref offset);
                break;
            case EmitState.BlockMappingValue:
                break;
            case EmitState.FlowMappingValue:
                break;
            case EmitState.FlowMappingKey:
                flowMapKeySerializer.BeginScalar(output, ref offset);
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
                blockSequenceEntrySerializer.EndScalar(output, ref offset);
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
                flowSequenceEntrySerializer.EndScalar(output, ref offset);
                break;
            case EmitState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(StateStack.Current.ToString());
        }
        Writer.Advance(offset);
    }
}
