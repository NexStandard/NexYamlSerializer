using NexVYaml;
using Stride.Core;
using System;
using System.Buffers.Text;

namespace NexYaml.Serialization.Formatters;

public class GuidFormatter : YamlSerializer<Guid>
{
    public static readonly GuidFormatter Instance = new();

    public override void Write(IYamlWriter stream, Guid value, DataStyle style)
    {
        // nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
        Span<byte> buf = stackalloc byte[64];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            stream.Write(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlException($"Cannot serialize {value}");
        }
    }

    public override void Read(IYamlReader parser, ref Guid value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
              Utf8Parser.TryParse(span, out Guid guid, out var bytesConsumed) &&
              bytesConsumed == span.Length)
        {
            parser.Move();
            value = guid;
            return;
        }
    }
}
