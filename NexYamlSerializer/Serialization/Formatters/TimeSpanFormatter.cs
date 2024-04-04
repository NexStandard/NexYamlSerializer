#nullable enable
using System;
using System.Buffers.Text;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class TimeSpanFormatter : YamlSerializer<TimeSpan>,IYamlFormatter<TimeSpan>
{
    public static readonly TimeSpanFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, TimeSpan value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        var buf = context.GetBuffer64();
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot serialize a value: {value}");
        }
    }

    public override TimeSpan Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out TimeSpan timeSpan, out var bytesConsumed) &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return timeSpan;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of TimeSpan : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref IYamlStream stream, TimeSpan value, DataStyle style = DataStyle.Normal)
    {
        var buf = stream.SerializeContext.GetBuffer64();
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            stream.Emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot serialize a value: {value}");
        }
    }
}
