using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;
using System.Text;

namespace NexYaml.Serializers;

public class Int64Serializer : YamlSerializer<long>
{
    public static readonly Int64Serializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, long value, DataStyle style, in WriteContext context)
    {
        return context.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref long value, ref ParseResult result)
    {
        stream.TryGetScalarAsSpan(out var span);
        stream.Move();

        if (long.TryParse(span, CultureInfo.InvariantCulture, out var temp))
        {
            value = temp;
        }
        else if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith(YamlCodes.HexPrefix))
        {
            var slice = span[YamlCodes.HexPrefix.Length..];
            if (Utf8Parser.TryParse(slice, out long hexVal, out var bytesConsumedHex, 'x') &&
                   bytesConsumedHex == slice.Length)
            {
                value = hexVal;
            }
        }
        else if (span.Length > YamlCodes.HexPrefixNegative.Length && span.StartsWith(YamlCodes.HexPrefixNegative))
        {
            var slice = span[YamlCodes.HexPrefixNegative.Length..];
            if (Utf8Parser.TryParse(slice, out long negativeHexVal, out var bytesConsumedHex, 'x') && bytesConsumedHex == slice.Length)
            {
                value = -negativeHexVal;
            }
        }
        else
        {
            YamlException.ThrowExpectedTypeParseException(typeof(long), Encoding.UTF8.GetString(span), stream.CurrentMarker);
        }
    }
}
