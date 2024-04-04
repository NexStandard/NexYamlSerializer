#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class SByteFormatter : YamlSerializer<sbyte>, IYamlFormatter<sbyte>
{
    public static readonly SByteFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, sbyte value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt32(value);
    }

    public override sbyte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return checked((sbyte)result);
    }

    public override void Serialize(ref IYamlStream stream, sbyte value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
