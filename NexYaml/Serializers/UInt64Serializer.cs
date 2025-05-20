using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class UInt64Serializer : YamlSerializer<ulong>
{
    public static readonly UInt64Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, ulong value, DataStyle style)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<ulong> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && ulong.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(ulong), span, stream.CurrentMarker);
        return new(default(ulong));
    }
}
