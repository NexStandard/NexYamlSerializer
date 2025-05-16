using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : YamlSerializer<string?>
{
    public static readonly NullableStringSerializer Instance = new();
    public override void Write<X>(WriteContext<X> context, string? value, DataStyle style)
    {
        context.Writer.WriteString(context,value, style);
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

