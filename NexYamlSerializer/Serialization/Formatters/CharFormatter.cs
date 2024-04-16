#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class CharFormatter : YamlSerializer<char>
{
    public static readonly CharFormatter Instance = new();

    public override char Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return checked((char)result);
    }

    public override void Serialize(ISerializationWriter stream, char value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
