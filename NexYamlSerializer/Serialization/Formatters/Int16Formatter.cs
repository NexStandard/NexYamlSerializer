#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class Int16Formatter : IYamlFormatter<short>
    {
        public static readonly Int16Formatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, short value, YamlSerializationContext context)
        {
            emitter.WriteInt32(value);
        }

        public short Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsInt32();
            parser.Read();
            return checked((short)result);
        }
    }
}
