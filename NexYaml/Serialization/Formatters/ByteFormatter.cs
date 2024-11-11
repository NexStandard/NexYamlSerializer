using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serialization.Formatters;

public class ByteFormatter : YamlSerializer<byte>
{
    public static readonly ByteFormatter Instance = new();

    public override void Write(IYamlWriter stream, byte value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref byte value, ref ParseResult parseResult)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (uint.TryParse(span, CultureInfo.InvariantCulture, out var result))
            {
                value = checked((byte)result);
                parser.Move();
                return;
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out value, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = checked((byte)result);
                    parser.Move();
                    return;
                }
            }
        }
    }
}