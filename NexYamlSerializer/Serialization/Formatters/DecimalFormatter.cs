#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System.Buffers.Text;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class DecimalFormatter : YamlSerializer<decimal>
{
    public static readonly DecimalFormatter Instance = new();

    protected override void Write(SerializationWriter stream, decimal value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref decimal value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
                   Utf8Parser.TryParse(span, out decimal val, out var bytesConsumed) &&
                   bytesConsumed == span.Length)
        {
            parser.Read();
            value = val;
            return;
        }
        throw new YamlException($"Cannot detect a scalar value of decimal : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }
}
