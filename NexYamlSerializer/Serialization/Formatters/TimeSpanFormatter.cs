#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers.Text;

namespace NexVYaml.Serialization;

public class TimeSpanFormatter : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanFormatter Instance = new();

    public override void Write(IYamlWriter stream, TimeSpan value, DataStyle style)
    {
        Span<byte> buf = stackalloc byte[32];
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            ReadOnlySpan<byte> bytes = buf[..bytesWritten];
            stream.Write(bytes);
        }
        else
        {
            throw new YamlException($"Cannot serialize a value: {value}");
        }
    }

    public override void Read(IYamlReader parser, ref TimeSpan value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
               Utf8Parser.TryParse(span, out TimeSpan timeSpan, out var bytesConsumed) &&
               bytesConsumed == span.Length)
        {
            parser.Move();
            value = timeSpan;
            return;
        }
    }
}
