#nullable enable
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization
{
    public class DateTimeFormatter : IYamlFormatter<DateTime>
    {
        public static readonly DateTimeFormatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, DateTime value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            // 2017-06-12T12:30:45.1234578+00:00
            // Span<byte> buf = stackalloc byte[29];
            var buf = context.GetBuffer64();
            if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
            {
                emitter.WriteScalar(buf[..bytesWritten]);
            }
            else
            {
                throw new YamlSerializerException($"Cannot format {value}");
            }
        }

        public DateTime Deserialize(ref YamlParser parser, YamlDeserializationContext context)
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
    }
}

