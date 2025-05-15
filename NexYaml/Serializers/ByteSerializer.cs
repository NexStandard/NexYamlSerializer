using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class ByteSerializer : YamlSerializer<byte>
{
    public static readonly ByteSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, byte value, DataStyle style)
    {
        Span<char> span = stackalloc char[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override void Read(IYamlReader stream, ref byte value, ref ParseResult parseResult)
    {
        if (stream.TryGetScalarAsSpan(out var span))
        {
            if (uint.TryParse(span, CultureInfo.InvariantCulture, out var result))
            {
                value = checked((byte)result);
                stream.Move();
                return;
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out value, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = checked((byte)result);
                    stream.Move();
                    return;
                }
            }
        }
    }
}
