using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : YamlSerializer<string?>
{
    public static readonly NullableStringSerializer Instance = new();
    public override WriteContext Write(IYamlWriter stream, string? value, DataStyle style, in WriteContext context)
    {
        return context.Write(value);
    }

    public override void Read(IYamlReader stream, ref string? value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsString(out value))
        {
            stream.Move(ParseEventType.Scalar);
            return;
        }
        value = null;
    }
}

