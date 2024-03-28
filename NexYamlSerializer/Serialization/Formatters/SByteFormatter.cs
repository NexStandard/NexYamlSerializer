#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class SByteFormatter : IYamlFormatter<sbyte>
{
    public static readonly SByteFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, sbyte value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt32(value);
    }

    public sbyte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return checked((sbyte)result);
    }
}
