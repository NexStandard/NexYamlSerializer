#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int32Formatter : YamlSerializer<int>
{
    public static readonly Int32Formatter Instance = new();

    public override int Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return result;
    }

    public override void Serialize(ISerializationWriter stream, int value, DataStyle style)
    {
        stream.Serialize(ref value);
    }
}
