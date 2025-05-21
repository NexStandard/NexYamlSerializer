using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, TimeSpan value, DataStyle style)
    {
        context.WriteString(value.ToString());
    }

    public override ValueTask<TimeSpan> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && TimeSpan.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(TimeSpan), span, stream.CurrentMarker);
    }
}
