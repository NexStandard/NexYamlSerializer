#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization
{
    public class NullableStringFormatter : IYamlFormatter<string?>
    {
        public static readonly NullableStringFormatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, string? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            if (value == null)
            {
                emitter.WriteNull();
            }
            else
            {
                emitter.WriteString(value);
            }
        }

        public string? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return null;
            }
            return parser.ReadScalarAsString();
        }
    }
}

