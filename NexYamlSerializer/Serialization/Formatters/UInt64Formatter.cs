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

public class UInt64Formatter : YamlSerializer<ulong>
{
    public static readonly UInt64Formatter Instance = new();

    public override void Write(IYamlWriter stream, ulong value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref ulong value)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (ulong.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = temp;
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out ulong temp2, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = temp2;
                }
            }
        }
        parser.Move();
    }
}
