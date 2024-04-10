#nullable enable
using System.Buffers.Text;
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class DecimalFormatter : YamlSerializer<decimal>, IYamlFormatter<decimal>
{
    public static readonly DecimalFormatter Instance = new();

    public override decimal Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
            Utf8Parser.TryParse(span, out decimal value, out var bytesConsumed) &&
            bytesConsumed == span.Length)
        {
            parser.Read();
            return value;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of decimal : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref ISerializationWriter stream, decimal value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
