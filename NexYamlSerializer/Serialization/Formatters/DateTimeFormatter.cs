#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;

namespace NexVYaml.Serialization;

public class DateTimeFormatter : YamlSerializer<DateTime>
{
    public static readonly DateTimeFormatter Instance = new();
    /// <summary>
    /// See DateTime source which sets FormatOMaxLength to 33
    /// https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Globalization/DateTimeFormat.cs,135
    /// </summary>
    private const int FormatOMaxLength = 33;
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

    public override void Serialize(ISerializationWriter stream, DateTime value, DataStyle style)
    {
        
        Span<byte> buf = stackalloc byte[FormatOMaxLength];
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

