using NexVYaml.Parser;
using NexYamlSerializer.Parser;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexVYaml.Serialization;

public class Int32Formatter : YamlSerializer<int>
{
    public static readonly Int32Formatter Instance = new();

    public override void Write(IYamlWriter stream, int value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref int value)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (int.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = temp;
                parser.Move();
                return;
            }

            if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out int hexTemp, out var bytesConsumed1, 'x') &&
                       bytesConsumed1 == hexNumber.Length)
                {
                    value = hexTemp;
                    parser.Move();
                    return;
                }
            }

            if (FormatHelper.TryDetectHexNegative(span, out hexNumber) &&
                Utf8Parser.TryParse(hexNumber, out int negativeHexTemp, out var bytesConsumed, 'x') &&
                bytesConsumed == hexNumber.Length)
            {
                value = negativeHexTemp;
                value *= -1;
                parser.Move();
                return;
            }
        }
    }
}
