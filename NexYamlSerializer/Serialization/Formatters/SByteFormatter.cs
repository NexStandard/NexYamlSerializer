#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class SByteFormatter : YamlSerializer<sbyte>
{
    public static readonly SByteFormatter Instance = new();

    public override sbyte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return checked((sbyte)result);
    }

    public override void Serialize(ISerializationWriter stream, sbyte value, DataStyle style)
    {
        stream.Serialize(ref value);
    }
}
