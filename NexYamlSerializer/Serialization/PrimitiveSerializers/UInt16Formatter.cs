#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt16Formatter : YamlSerializer<ushort>, IYamlFormatter<ushort>
{
    public static readonly UInt16Formatter Instance = new();

    public override ushort Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return checked((ushort)result);
    }

    public override void Serialize(ref ISerializationWriter stream, ushort value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
