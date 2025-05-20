using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class ByteSerializer : YamlSerializer<byte>
{
    public static readonly ByteSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, byte value, DataStyle style)
    {
        Span<char> span = stackalloc char[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<byte> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && byte.TryParse(span, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(DateTime), span, stream.CurrentMarker);
        return new(0);
    }
}
