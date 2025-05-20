using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;
using System.Globalization;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : YamlSerializer<TimeSpan>
{
    public static readonly TimeSpanSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, TimeSpan value, DataStyle style)
    {
        Span<char> buf = stackalloc char[32];
        
        if (value.TryFormat(buf, out var bytesWritten))
        {
            context.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlException($"Cannot serialize a value: {value}");
        }
    }

    public override ValueTask<TimeSpan> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && TimeSpan.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move();
            return new(value);
        }
        stream.Move();
        YamlException.ThrowExpectedTypeParseException(typeof(TimeSpan), span, stream.CurrentMarker);
        return new(default(TimeSpan));
    }
}
