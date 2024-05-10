#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers.Text;

namespace NexVYaml.Serialization;

public class TimeSpanFormatter : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanFormatter Instance = new();

    protected override void Write(SerializationWriter stream, TimeSpan value, DataStyle style)
    {
        Span<byte> buf = stackalloc byte[32];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            ReadOnlySpan<byte> bytes = buf[..bytesWritten];
            stream.Serialize(ref bytes);
        }
        else
        {
            throw new YamlSerializerException($"Cannot serialize a value: {value}");
        }
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref TimeSpan value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
               Utf8Parser.TryParse(span, out TimeSpan timeSpan, out var bytesConsumed) &&
               bytesConsumed == span.Length)
        {
            parser.Read();
            value = timeSpan;
            return;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of TimeSpan : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }
}
