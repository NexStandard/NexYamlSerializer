using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Globalization;

namespace NexYaml.Serializers;

public class Float32Serializer : YamlSerializer<float>
{
    public static readonly Float32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, float value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<float> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && float.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(float), span, stream.CurrentMarker);
        return new(default(float));
    }
}
