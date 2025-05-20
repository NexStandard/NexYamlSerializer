using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;
using System.Text;

namespace NexYaml.Serializers;

public class Int64Serializer : YamlSerializer<long>
{
    public static readonly Int64Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, long value, DataStyle style)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<long> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && long.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(long), span, stream.CurrentMarker);
        return new(default(long));
    }
}
