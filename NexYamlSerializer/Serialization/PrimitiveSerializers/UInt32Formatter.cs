#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt32Formatter : YamlSerializer<uint>, IYamlFormatter<uint>
{
    public static readonly UInt32Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, uint value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteUInt32(value);
    }

    public override uint Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return result;
    }

    public override void Serialize(ref IYamlStream stream, uint value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
