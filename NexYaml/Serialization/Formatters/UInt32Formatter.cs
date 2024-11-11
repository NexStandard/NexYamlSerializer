using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serialization.Formatters;

public class UInt32Formatter : YamlSerializer<uint>
{
    public static readonly UInt32Formatter Instance = new();

    public override void Write(IYamlWriter stream, uint value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref uint value, ref ParseResult result)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (uint.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = temp;
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out uint temp2, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = temp2;
                }
            }
        }
        parser.Move();
    }
}