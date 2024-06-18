#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;
using System.Buffers.Text;
using NexYamlSerializer.Parser;

namespace NexVYaml.Serialization;

public class Int16Formatter : YamlSerializer<short>
{
    public static readonly Int16Formatter Instance = new();

    protected override void Write(IYamlWriter stream, short value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref short value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            if (int.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = checked((short)temp);
                parser.Read();
                return;
            }

            if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if(Utf8Parser.TryParse(hexNumber, out int hexTemp, out var bytesConsumed1, 'x') &&
                       bytesConsumed1 == hexNumber.Length)
                {
                    value = checked((short)hexTemp);
                    parser.Read();
                    return;
                }
            }

            if (FormatHelper.TryDetectHexNegative(span, out hexNumber) &&
                Utf8Parser.TryParse(hexNumber,  out int negativeHexTemp, out var bytesConsumed, 'x') &&
                bytesConsumed == hexNumber.Length)
            {
                value = checked((short)negativeHexTemp);
                value *= -1;
                parser.Read();
                return;
            }
        }
    }
}
