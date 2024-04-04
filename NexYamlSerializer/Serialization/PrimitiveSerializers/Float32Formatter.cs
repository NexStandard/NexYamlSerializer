#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Float32Formatter : YamlSerializer<float>,IYamlFormatter<float>
{
    public static readonly Float32Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, float value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteFloat(value);
    }

    public override float Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsFloat();
        parser.Read();
        return result;
    }

    public override void Serialize(ref IYamlStream stream, float value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
