using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class UInt16Serializer : YamlSerializer<ushort>
{
    public static readonly UInt16Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, ushort value, DataStyle style)
    {
        Span<char> span = stackalloc char[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override void Read(IYamlReader stream, ref ushort value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
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
        stream.Move();
    }
}
