#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class Int32Formatter : IYamlFormatter<int>
    {
        public static readonly Int32Formatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, int value, YamlSerializationContext context)
        {
            emitter.WriteInt32(value);
        }

        public int Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsInt32();
            parser.Read();
            return result;
        }
    }
}
