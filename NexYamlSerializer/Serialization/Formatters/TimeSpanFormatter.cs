#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers.Text;

namespace NexVYaml.Serialization;

public class TimeSpanFormatter : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanFormatter Instance = new();

    public override TimeSpan Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out TimeSpan timeSpan, out var bytesConsumed) &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return timeSpan;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of TimeSpan : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ISerializationWriter stream, TimeSpan value, DataStyle style = DataStyle.Normal)
    {
        Span<byte> buf = stackalloc byte[64];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            stream.Serialize(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot serialize a value: {value}");
        }
    }
}
