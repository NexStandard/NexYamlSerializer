using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serialization.Formatters;

public class Int16Formatter : YamlSerializer<short>
{
    public static readonly Int16Formatter Instance = new();

    public override void Write(IYamlWriter stream, short value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref short value, ref ParseResult result)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (int.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = checked((short)temp);
                parser.Move();
                return;
            }

            if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out int hexTemp, out var bytesConsumed1, 'x') &&
                       bytesConsumed1 == hexNumber.Length)
                {
                    value = checked((short)hexTemp);
                    parser.Move();
                    return;
                }
            }

            if (FormatHelper.TryDetectHexNegative(span, out hexNumber) &&
                Utf8Parser.TryParse(hexNumber, out int negativeHexTemp, out var bytesConsumed, 'x') &&
                bytesConsumed == hexNumber.Length)
            {
                value = checked((short)negativeHexTemp);
                value *= -1;
                parser.Move();
                return;
            }
        }
    }
}
