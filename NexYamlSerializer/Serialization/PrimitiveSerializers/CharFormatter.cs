#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class CharFormatter : YamlSerializer<char>, IYamlFormatter<char>
{
    public static readonly CharFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, char value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt32(value);
    }

    public override char Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return checked((char)result);
    }

    public override void Serialize(ref IYamlStream stream, char value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
