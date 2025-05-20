using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class GuidSerializer : YamlSerializer<Guid>
{
    public static readonly GuidSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, Guid value, DataStyle style)
    {
        // nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
        Span<char> buf = stackalloc char[36];
        if (value.TryFormat(buf, out var bytesWritten))
        {
            context.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlException($"Cannot serialize {value}");
        }
    }

    public override ValueTask<Guid> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && Guid.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new (value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(Guid), span, stream.CurrentMarker);
        return default;
    }
}
