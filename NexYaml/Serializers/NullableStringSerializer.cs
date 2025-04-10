using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : YamlSerializer<string?>
{
    public static readonly NullableStringSerializer Instance = new();
    public override void Write(IYamlWriter stream, string? value, DataStyle style)
    {
        stream.Write(value!);
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

