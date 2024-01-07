#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class Float32Formatter : IYamlFormatter<float>
    {
        public static readonly Float32Formatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, float value, YamlSerializationContext context)
        {
            emitter.WriteFloat(value);
        }

        public float Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsFloat();
            parser.Read();
            return result;
        }
    }
}
