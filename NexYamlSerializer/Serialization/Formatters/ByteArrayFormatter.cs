#nullable enable
using System;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public sealed class ByteArrayFormatter : IYamlFormatter<byte[]?>
    {
        public static readonly IYamlFormatter<byte[]?> Instance = new ByteArrayFormatter();

        public void Serialize(ref Utf8YamlEmitter emitter, byte[]? value, YamlSerializationContext context)
        {
            if (value == null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.WriteString(
                Convert.ToBase64String(value, Base64FormattingOptions.None),
                ScalarStyle.Plain);
        }

        public byte[]? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return null;
            }

            var str = parser.ReadScalarAsString();
            return Convert.FromBase64String(str!);
        }
    }
}

