using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serialization.Formatters;

public class UInt16Formatter : YamlSerializer<ushort>
{
    public static readonly UInt16Formatter Instance = new();

    public override void Write(IYamlWriter stream, ushort value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref ushort value)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (uint.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = checked((ushort)temp);
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out uint temp2, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = checked((ushort)temp2);
                }
            }
        }
        parser.Move();
    }
}
