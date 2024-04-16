#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers;
using System.Buffers.Text;

namespace NexVYaml.Serialization;

public class DateTimeOffsetFormatter : YamlSerializer<DateTimeOffset>, IYamlFormatter<DateTimeOffset>
{
    public static readonly DateTimeOffsetFormatter Instance = new();

    public override DateTimeOffset Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out DateTimeOffset value, out var bytesConsumed) &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return value;
        }

        throw new YamlSerializerException($"Cannot detect a scalar value of DateTimeOffset : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref ISerializationWriter stream, DateTimeOffset value, DataStyle style = DataStyle.Normal)
    {
        Span<byte> buf = stackalloc byte[64];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
        {
            stream.Serialize(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot format {value}");
        }
    }
}
