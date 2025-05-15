using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, TimeSpan value, DataStyle style)
    {
        Span<char> buf = stackalloc char[32];
        
        if (value.TryFormat(buf, out var bytesWritten))
        {
            context.WriteScalar(buf[..bytesWritten]);
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
