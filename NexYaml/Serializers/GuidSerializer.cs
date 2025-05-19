using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;

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
    public override ValueTask<Guid> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsSpan(out var span) &&
              Utf8Parser.TryParse(span, out Guid guid, out var bytesConsumed) &&
              bytesConsumed == span.Length)
        {
            stream.Move();
            return new ValueTask<Guid>(guid);
        }
        else
        {
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(Guid), text, stream.CurrentMarker);
            stream.Move();
            return new ValueTask<Guid>(Guid.Empty);
        }
    }
}
