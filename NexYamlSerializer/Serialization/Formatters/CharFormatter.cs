#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class CharFormatter : YamlSerializer<char>
{
    public static readonly CharFormatter Instance = new();

    public override void Serialize(ISerializationWriter stream, char value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref char value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((char)result);
    }
}
