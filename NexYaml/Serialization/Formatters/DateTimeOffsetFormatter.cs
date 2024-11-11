using System.Buffers;
using System.Buffers.Text;
using NexYaml.Parser;
using Stride.Core;
using NexYaml.Core;

namespace NexYaml.Serialization.Formatters;

public class DateTimeOffsetFormatter : YamlSerializer<DateTimeOffset>
{
    public static readonly DateTimeOffsetFormatter Instance = new();

    public override void Write(IYamlWriter stream, DateTimeOffset value, DataStyle style)
    {
        Span<byte> buf = stackalloc byte[64];
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

    public override void Read(IYamlReader parser, ref DateTimeOffset value, ref ParseResult result)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
             Utf8Parser.TryParse(span, out DateTimeOffset val, out var bytesConsumed) &&
             bytesConsumed == span.Length)
        {
            parser.Move();
            value = val;
        }
    }
}
