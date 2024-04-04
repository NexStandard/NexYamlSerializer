#nullable enable
using System;
using System.Buffers;
using System.Buffers.Text;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class DateTimeOffsetFormatter : YamlSerializer<DateTimeOffset>,IYamlFormatter<DateTimeOffset>
{
    public static readonly DateTimeOffsetFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, DateTimeOffset value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        var buf = context.GetBuffer64();
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
        {
            emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot format {value}");
        }
    }

    public override DateTimeOffset Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out DateTimeOffset value, out var bytesConsumed) &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return value;
        }

        throw new YamlSerializerException($"Cannot detect a scalar value of DateTimeOffset : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref IYamlStream stream, DateTimeOffset value, DataStyle style = DataStyle.Normal)
    {
        var buf = stream.SerializeContext.GetBuffer64();
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten, new StandardFormat('O')))
        {
            stream.Emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot format {value}");
        }
    }
}
