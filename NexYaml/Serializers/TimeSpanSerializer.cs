using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanSerializer Instance = new();

    public override void Write(IYamlWriter stream, TimeSpan value, DataStyle style)
    {
        Span<byte> buf = stackalloc byte[32];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            ReadOnlySpan<byte> bytes = buf[..bytesWritten];
            stream.Write(bytes);
        }
        else
        {
            throw new YamlException($"Cannot serialize a value: {value}");
        }
    }

    public override void Read(IYamlReader stream, ref TimeSpan value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span) &&
               Utf8Parser.TryParse(span, out TimeSpan timeSpan, out var bytesConsumed) &&
               bytesConsumed == span.Length)
        {
            stream.Move();
            value = timeSpan;
            return;
        }
    }
}
