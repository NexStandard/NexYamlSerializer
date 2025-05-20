using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
namespace NexYaml.Serializers;

public class BooleanSerializer : YamlSerializer<bool>
{
    public static readonly BooleanSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, bool value, DataStyle style)
    {
        context.WriteScalar(value ? ['t', 'r', 'u', 'e'] : [ 'f', 'a', 'l', 's', 'e' ]);
    }

    public override ValueTask<bool> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && bool.TryParse(span, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(DateTime), span, stream.CurrentMarker);
        return new(false);
    }
}
