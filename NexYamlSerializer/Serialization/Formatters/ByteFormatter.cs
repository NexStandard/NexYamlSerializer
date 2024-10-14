#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;
using System.Buffers.Text;
using NexYamlSerializer.Parser;

namespace NexVYaml.Serialization;

public class ByteFormatter : YamlSerializer<byte>
{
    public static readonly ByteFormatter Instance = new();

    protected override void Write(IYamlWriter stream, byte value, DataStyle style)
    {
        stream.Write(value, style);
    }

    protected override void Read(IYamlReader parser, ref byte value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            if (uint.TryParse(span, CultureInfo.InvariantCulture, out var result))
            {
                value = checked((byte)result);
                parser.Read();
                return;
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if(Utf8Parser.TryParse(hexNumber, out value, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = checked((byte)result);
                    parser.Read();
                    return;
                }
            }
        }
    }
}
