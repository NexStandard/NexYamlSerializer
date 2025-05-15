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

    public override void Read(IYamlReader stream, ref char value, ref ParseResult parseResult)
    {
        if (stream.TryGetScalarAsString(out var result))
        {
            if (result is not null && result.Length == 1)
            {
                if (result.Length == 1)
                {
                    value = result[0];
                    stream.Move(ParseEventType.Scalar);
                }
            }
            else
            {
                stream.TryGetScalarAsString(out var text);
                YamlException.ThrowExpectedTypeParseException(typeof(int), text, stream.CurrentMarker);
            }
        }

    }
}
