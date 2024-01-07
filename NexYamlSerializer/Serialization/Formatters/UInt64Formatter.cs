#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class UInt64Formatter : IYamlFormatter<ulong>
    {
        public static readonly UInt64Formatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, ulong value, YamlSerializationContext context)
        {
            emitter.WriteUInt64(value);
        }

        public ulong Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsUInt64();
            parser.Read();
            return result;
        }
    }
}
