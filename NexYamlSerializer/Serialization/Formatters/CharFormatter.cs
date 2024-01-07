#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class CharFormatter : IYamlFormatter<char>
    {
        public static readonly CharFormatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, char value, YamlSerializationContext context)
        {
            emitter.WriteInt32(value);
        }

        public char Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsUInt32();
            parser.Read();
            return checked((char)result);
        }
    }
}
