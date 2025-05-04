using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class SByteSerializer : YamlSerializer<sbyte>
{
    public static readonly SByteSerializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, sbyte value, DataStyle style, in WriteContext context)
    {
        return context.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref sbyte value, ref ParseResult parseResult)
    {
        if (stream.TryGetScalarAsSpan(out var span))
        {
            if (int.TryParse(span, CultureInfo.InvariantCulture, out var result))
            {
                value = checked((sbyte)result);
                stream.Move();
                return;
            }
            else if (FormatHelper.TryDetectHex(span, out var hexNumber))
            {
                if (Utf8Parser.TryParse(hexNumber, out value, out var bytesConsumed, 'x') &&
                       bytesConsumed == hexNumber.Length)
                {
                    value = checked((sbyte)result);
                    stream.Move();
                    return;
                }
            }
        }
    }
}
