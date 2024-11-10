using NexVYaml;
using NexVYaml.Parser;
using Stride.Core;
using System.Buffers.Text;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class DecimalFormatter : YamlSerializer<decimal>
{
    public static readonly DecimalFormatter Instance = new();

    public override void Write(IYamlWriter stream, decimal value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref decimal value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
                   Utf8Parser.TryParse(span, out decimal val, out var bytesConsumed) &&
                   bytesConsumed == span.Length)
        {
            value = val;
            parser.Move();
            return;
        }
    }
}
