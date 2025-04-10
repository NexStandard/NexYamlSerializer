using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;

namespace NexYaml.Serializers;

public class DecimalSerializer : YamlSerializer<decimal>
{
    public static readonly DecimalSerializer Instance = new();

    public override void Write(IYamlWriter stream, decimal value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref decimal value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span) &&
                   Utf8Parser.TryParse(span, out decimal val, out var bytesConsumed) &&
                   bytesConsumed == span.Length)
        {
            value = val;
            stream.Move();
            return;
        }
    }
}
