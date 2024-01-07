#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class UInt16Formatter : IYamlFormatter<ushort>
    {
        public static readonly UInt16Formatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, ushort value, YamlSerializationContext context)
        {
            emitter.WriteUInt32(value);
        }

        public ushort Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsUInt32();
            parser.Read();
            return checked((ushort)result);
        }
    }
}
