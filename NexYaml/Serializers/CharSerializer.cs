using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : YamlSerializer<char>
{
    public static readonly CharSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, char value, DataStyle style)
    {
        context.WriteScalar(['\'', value, '\'']);
    }

    public override ValueTask<char> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && char.TryParse(span, out var value) && span.Length == 1)
        {
            value = span[0];
            stream.Move(ParseEventType.Scalar);
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(DateTime), span, stream.CurrentMarker);
    }
}
