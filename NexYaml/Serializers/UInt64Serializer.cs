using NexYaml.Parser;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class UInt64Serializer : YamlSerializer<ulong>
{
    public static readonly UInt64Serializer Instance = new();

    public override void Write(IYamlWriter stream, ulong value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref ulong value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
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
        stream.Move();
    }
}
