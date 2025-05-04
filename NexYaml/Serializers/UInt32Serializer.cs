using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class UInt32Serializer : YamlSerializer<uint>
{
    public static readonly UInt32Serializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, uint value, DataStyle style, in WriteContext context)
    {
        return context.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref uint value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
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
        stream.Move();
    }
}
