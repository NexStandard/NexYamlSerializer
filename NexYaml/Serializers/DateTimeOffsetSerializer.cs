using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers;
using System.Buffers.Text;

namespace NexYaml.Serializers;

public class DateTimeOffsetSerializer : YamlSerializer<DateTimeOffset>
{
    public static readonly DateTimeOffsetSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, DateTimeOffset value, DataStyle style)
    {
        context.WriteType(value.ToString(), style);
    }

    public override void Read(IYamlReader stream, ref DateTimeOffset value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span) &&
             Utf8Parser.TryParse(span, out DateTimeOffset val, out var bytesConsumed) &&
             bytesConsumed == span.Length)
        {
            stream.Move();
            value = val;
        }
        else
        {
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(DateTimeOffset), text, stream.CurrentMarker);
        }
    }
}
