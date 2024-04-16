#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt32Formatter : YamlSerializer<uint>
{
    public static readonly UInt32Formatter Instance = new();

    public override uint Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return result;
    }

    public override void Serialize(ref ISerializationWriter stream, uint value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
