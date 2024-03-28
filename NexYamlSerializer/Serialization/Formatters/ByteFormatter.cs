#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class ByteFormatter : IYamlFormatter<byte>
{
    public static readonly ByteFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, byte value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt32(value);
    }

    public byte Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return checked((byte)result);
    }
}
