using NexVYaml.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using System;

namespace NexYamlSerializer.Emitter;
public static class Utf8StreamExtensions
{
    public static EmitState PreviousState(this IUTF8Stream stream)
    {
        return stream.Previous.State;
    }
    public static IEmitter Map(this IUTF8Stream stream, EmitState state)
    {
        return stream.EmitterFactory.Map(state);
    }

    public static IUTF8Stream WriteRaw(this IUTF8Stream stream, byte value)
    {
        stream.WriteRaw([value]);
        return stream;
    }
    public static IUTF8Stream WriteRaw(this IUTF8Stream stream, ReadOnlySpan<byte> value, bool lineBreak)
    {
        stream.WriteRaw(value);
        if (lineBreak)
        {
            stream.WriteNewLine();
        }
        return stream;
    }
    public static IUTF8Stream WriteFlowSequenceSeparator(this IUTF8Stream stream)
    {
        stream.WriteRaw([',', ' ']);
        return stream;
    }
    public static IUTF8Stream WriteFlowSequenceStart(this IUTF8Stream stream)
    {
        stream.WriteRaw(['[']);
        return stream;
    }
    public static IUTF8Stream WriteFlowSequenceEnd(this IUTF8Stream stream)
    {
        stream.WriteRaw([']']);
        return stream;
    }
    public static IUTF8Stream WriteMappingKeyFooter(this IUTF8Stream stream)
    {
        stream.WriteRaw(": ");
        return stream;
    }
    public static IUTF8Stream WriteEmptyFlowSequence(this IUTF8Stream stream)
    {
        stream.WriteRaw(['[', ']']);
        return stream;
    }
    public static IUTF8Stream WriteEmptyFlowMapping(this IUTF8Stream stream)
    {
        stream.WriteRaw(['{', '}']);
        return stream;
    }
    public static IUTF8Stream WriteFlowMappingStart(this IUTF8Stream stream)
    {
        stream.WriteRaw(['{', ' ']);
        return stream;
    }
    public static IUTF8Stream WriteFlowMappingEnd(this IUTF8Stream stream)
    {
        stream.WriteRaw([' ', '}']);
        return stream;
    }
    public static IUTF8Stream WriteSpace(this IUTF8Stream stream)
    {
        stream.WriteRaw([' ']);
        return stream;
    }
    public static IUTF8Stream WriteSequenceSeparator(this IUTF8Stream stream)
    {
        stream.WriteRaw(['-', ' ']);
        return stream;
    }
    public static IUTF8Stream WriteNewLine(this IUTF8Stream stream)
    {
        stream.WriteRaw((byte)'\n');
        return stream;
    }
}
