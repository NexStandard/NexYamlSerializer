using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class Int32Serializer : YamlSerializer<int>
{
    public static readonly Int32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, int value, DataStyle style)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<int> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && int.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(double), span, stream.CurrentMarker);
        return new(0);
    }
}
