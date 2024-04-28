#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Float32Formatter : YamlSerializer<float>
{
    public static readonly Float32Formatter Instance = new();

    public override float Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsFloat();
        parser.Read();
        return result;
    }

    public override void Serialize(ISerializationWriter stream, float value, DataStyle style)
    {
        stream.Serialize(ref value);
    }
}
