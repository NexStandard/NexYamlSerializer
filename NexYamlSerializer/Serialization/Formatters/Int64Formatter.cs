#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;
using NexYaml.Core;
using System.Buffers.Text;

namespace NexVYaml.Serialization;

public class Int64Formatter : YamlSerializer<long>
{
    public static readonly Int64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, long value, DataStyle style)
    {
        stream.Write(value, style);
    }

    protected override void Read(IYamlReader parser, ref long value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            if (long.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = temp;
            }

            else if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith(YamlCodes.HexPrefix))
            {
                var slice = span[YamlCodes.HexPrefix.Length..];
                if(Utf8Parser.TryParse(slice, out long hexVal, out var bytesConsumedHex, 'x') &&
                       bytesConsumedHex == slice.Length)
                {
                    value = hexVal;
                }
            }
            if (span.Length > YamlCodes.HexPrefixNegative.Length && span.StartsWith(YamlCodes.HexPrefixNegative))
            {
                var slice = span[YamlCodes.HexPrefixNegative.Length..];
                if (Utf8Parser.TryParse(slice, out long negativeHexVal, out var bytesConsumedHex, 'x') && bytesConsumedHex == slice.Length)
                {
                    value = -negativeHexVal;
                }
            }
        }
        parser.Move();
    }
}
