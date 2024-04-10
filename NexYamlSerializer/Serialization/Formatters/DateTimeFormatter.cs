#nullable enable
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class DateTimeFormatter : YamlSerializer<DateTime>,IYamlFormatter<DateTime>
{
    public static readonly DateTimeFormatter Instance = new();

    public override DateTime Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out DateTime dateTime, out var bytesConsumed, 'O') &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return dateTime;
        }

        // fallback
        if (DateTime.TryParse(parser.GetScalarAsString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
        {
            parser.Read();
            return dateTime;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of DateTime : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref ISerializationWriter stream, DateTime value, DataStyle style = DataStyle.Normal)
    {
        // 2017-06-12T12:30:45.1234578+00:00
        // Span<byte> buf = stackalloc byte[29];
        Span<byte> buf = stackalloc byte[64];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
        {
            stream.Emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot format {value}");
        }
    }
}

