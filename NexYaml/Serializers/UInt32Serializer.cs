using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class UInt32Serializer : YamlSerializer<uint>
{
    public static readonly UInt32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, uint value, DataStyle style)
    {
        Span<char> span = stackalloc char[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<uint> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && uint.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(uint), span, stream.CurrentMarker);
        return new(default(uint));
    }
}
