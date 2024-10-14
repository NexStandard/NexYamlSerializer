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

    protected override void Write(IYamlWriter stream, DateTime value, DataStyle style)
    {
        Span<byte> buf = stackalloc byte[FormatOMaxLength];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
        {
            ReadOnlySpan<byte> bytes = buf[..bytesWritten];
            stream.Write(bytes);
        }
        else
        {
            throw new YamlException($"Cannot format {value}");
        }
    }

    protected override void Read(IYamlReader parser, ref DateTime value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
      Utf8Parser.TryParse(span, out DateTime dateTime, out var bytesConsumed, 'O') &&
      bytesConsumed == span.Length)
        {
            parser.Read();
            value = dateTime;
            return;
        }
        // fallback
        if (parser.TryGetScalarAsString(out var scalarString))
        {
            if (DateTime.TryParse(scalarString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
            {
                parser.Read();
                value = dateTime;
                return;
            }
        }
        
    }
}

