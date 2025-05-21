using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class SByteSerializer : YamlSerializer<sbyte>
{
    public static readonly SByteSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, sbyte value, DataStyle style)
    {
        Span<char> span = stackalloc char[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<sbyte> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && sbyte.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(sbyte), span, stream.CurrentMarker);
    }
}
