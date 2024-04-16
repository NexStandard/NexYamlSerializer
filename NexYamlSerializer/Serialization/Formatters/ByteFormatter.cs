#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class ByteFormatter : YamlSerializer<byte>, IYamlFormatter<byte>
{
    public static readonly ByteFormatter Instance = new();

    public override byte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return checked((byte)result);
    }

    public override void Serialize(ref ISerializationWriter stream, byte value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
