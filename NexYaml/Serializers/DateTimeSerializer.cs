using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
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
            stream.Move(ParseEventType.Scalar);
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(DateTime), span, stream.CurrentMarker);
    }
    public override ValueTask<DateTime> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(DateTime.Parse(scalarScope.Value));
    }
}
