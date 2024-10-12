#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System.Globalization;
using System;
using System.Buffers.Text;
using NexYamlSerializer.Parser;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt16Formatter : YamlSerializer<ushort>
{
    public static readonly UInt16Formatter Instance = new();

    protected override void Write(IYamlWriter stream, ushort value, DataStyle style)
    {
        stream.Write(value, style);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ushort value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            if (uint.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = checked((ushort)temp);
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if(Utf8Parser.TryParse(hexNumber, out uint temp2, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = checked((ushort)temp2);
                }
            }
        }
        parser.Read();
    }
}
