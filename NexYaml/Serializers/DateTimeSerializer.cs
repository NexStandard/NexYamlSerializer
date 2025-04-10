using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class DateTimeSerializer : YamlSerializer<DateTime>
{
    public static readonly DateTimeSerializer Instance = new();
    /// <summary>
    /// See DateTime source which sets FormatOMaxLength to 33
    /// https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Globalization/DateTimeFormat.cs,135
    /// </summary>
    private const int FormatOMaxLength = 33;

    public override void Write(IYamlWriter stream, DateTime value, DataStyle style)
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

    public override void Read(IYamlReader stream, ref DateTime value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span) &&
      Utf8Parser.TryParse(span, out DateTime dateTime, out var bytesConsumed, 'O') &&
      bytesConsumed == span.Length)
        {
            stream.Move();
            value = dateTime;
            return;
        }
        // fallback
        if (stream.TryGetScalarAsString(out var scalarString))
        {
            if (DateTime.TryParse(scalarString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
            {
                stream.Move();
                value = dateTime;
                return;
            }
        }
    }
}

