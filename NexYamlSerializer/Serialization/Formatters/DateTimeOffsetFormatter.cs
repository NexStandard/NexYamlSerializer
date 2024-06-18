#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers;
using System.Buffers.Text;

namespace NexVYaml.Serialization;

public class DateTimeOffsetFormatter : YamlSerializer<DateTimeOffset>
{
    public static readonly DateTimeOffsetFormatter Instance = new();

    protected override void Write(IYamlWriter stream, DateTimeOffset value, DataStyle style)
    {
        Span<byte> buf = stackalloc byte[64];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
        {
            ReadOnlySpan<byte> bytes = buf[..bytesWritten];
            stream.Serialize(bytes);
        }
        else
        {
            throw new YamlException($"Cannot format {value}");
        }
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref DateTimeOffset value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
             Utf8Parser.TryParse(span, out DateTimeOffset val, out var bytesConsumed) &&
             bytesConsumed == span.Length)
        {
            parser.Read();
            value = val;
        }
    }
}
