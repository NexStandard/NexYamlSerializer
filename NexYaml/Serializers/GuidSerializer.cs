using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;

namespace NexYaml.Serializers;

public class GuidSerializer : YamlSerializer<Guid>
{
    public static readonly GuidSerializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, Guid value, DataStyle style, in WriteContext context)
    {
        // nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
        Span<byte> buf = stackalloc byte[64];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            return context.Write(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlException($"Cannot serialize {value}");
        }
    }

    public override void Read(IYamlReader stream, ref Guid value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span) &&
              Utf8Parser.TryParse(span, out Guid guid, out var bytesConsumed) &&
              bytesConsumed == span.Length)
        {
            stream.Move();
            value = guid;
            return;
        }
        else
        {
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(Guid), text, stream.CurrentMarker);
        }
    }
}
