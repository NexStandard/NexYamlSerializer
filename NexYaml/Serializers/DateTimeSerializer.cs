using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeSerializer : YamlSerializer<DateTime>
{
    public static YamlSerializer<DateTime> Instance = new DateTimeSerializer();
    public override void Write<X>(WriteContext<X> context, DateTime value, DataStyle style)
    {
        context.WriteType(value.ToString(), style);
    }

    public override ValueTask<DateTime> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && DateTime.TryParse(span, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(DateTime), span, stream.CurrentMarker);
    }
}