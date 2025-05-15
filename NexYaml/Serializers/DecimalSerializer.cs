using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class DecimalSerializer : YamlSerializer<decimal>
{
    public static readonly DecimalSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, decimal value, DataStyle style)
    {
        Span<char> span = stackalloc char[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
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
