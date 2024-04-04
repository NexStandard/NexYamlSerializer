#nullable enable
using System;
using System.Buffers.Text;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class GuidFormatter : YamlSerializer<Guid>, IYamlFormatter<Guid>
{
    public static readonly GuidFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, Guid value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        // nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
        var buf = context.GetBuffer64();
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot serialize {value}");
        }
    }

    public override Guid Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out Guid guid, out var bytesConsumed) &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return guid;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of Guid : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref IYamlStream stream, Guid value, DataStyle style = DataStyle.Normal)
    {
        // nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
        var buf = stream.SerializeContext.GetBuffer64();
        if (Utf8Formatter.TryFormat(value, buf, out var bytesWritten))
        {
            stream.Emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlSerializerException($"Cannot serialize {value}");
        }
    }
}
