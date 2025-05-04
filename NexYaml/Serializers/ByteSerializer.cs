using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class ByteSerializer : YamlSerializer<byte>
{
    public static readonly ByteSerializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, byte value, DataStyle style, in WriteContext context)
    {
        return context.Write(value, style);
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
