using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeOffsetSerializer : YamlSerializer<DateTimeOffset>
{
    public static readonly DateTimeOffsetSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, DateTimeOffset value, DataStyle style)
    {
        context.WriteType(value.ToString(), style);
    }

    public override ValueTask<DateTimeOffset> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && DateTimeOffset.TryParse(span, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(DateTimeOffset), span, stream.CurrentMarker);
    }
}
