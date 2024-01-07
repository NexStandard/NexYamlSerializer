#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class BooleanFormatter : IYamlFormatter<bool>
    {
        public static readonly BooleanFormatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, bool value, YamlSerializationContext context)
        {
            emitter.WriteBool(value);
        }

        public bool Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsBool();
            parser.Read();
            return result;
        }
    }
}
