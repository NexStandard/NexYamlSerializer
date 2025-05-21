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

    public override ValueTask<string?> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span))
        {
            stream.Move();
            return new(span);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(short), span, stream.CurrentMarker);
    }
}