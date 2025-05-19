using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class Int32Serializer : YamlSerializer<int>
{
    public static readonly Int32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, int value, DataStyle style)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override void Read(IYamlReader stream, ref int value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
        {
            if (int.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                value = temp;
                stream.Move();
                return;
            }

            if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out int hexTemp, out var bytesConsumed1, 'x') &&
                       bytesConsumed1 == hexNumber.Length)
                {
                    value = hexTemp;
                    stream.Move();
                    return;
                }
            }

            if (FormatHelper.TryDetectHexNegative(span, out hexNumber) &&
                Utf8Parser.TryParse(hexNumber, out int negativeHexTemp, out var bytesConsumed, 'x') &&
                bytesConsumed == hexNumber.Length)
            {
                value = negativeHexTemp;
                value *= -1;
                stream.Move();
                return;
            }
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(int), text, stream.CurrentMarker);
        }
    }
    public override ValueTask<int> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsSpan(out var span))
        {
            if (int.TryParse(span, CultureInfo.InvariantCulture, out var temp))
            {
                stream.Move();
                return new ValueTask<int>(temp);
            }

            if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out int hexTemp, out var bytesConsumed1, 'x') &&
                       bytesConsumed1 == hexNumber.Length)
                {
                    stream.Move();
                    return new ValueTask<int>(hexTemp);
                }
            }

            if (FormatHelper.TryDetectHexNegative(span, out hexNumber) &&
                Utf8Parser.TryParse(hexNumber, out int negativeHexTemp, out var bytesConsumed, 'x') &&
                bytesConsumed == hexNumber.Length)
            {
                var value = negativeHexTemp;
                value *= -1;
                stream.Move();
                return new ValueTask<int>(value);
            }
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(int), text, stream.CurrentMarker);
        }
        return new ValueTask<int>(0);
    }
}
