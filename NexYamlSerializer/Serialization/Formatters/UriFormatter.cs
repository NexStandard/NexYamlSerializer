#nullable enable
using System;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization
{
    public class UriFormatter : IYamlFormatter<Uri>
    {
        public static readonly UriFormatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, Uri value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            emitter.WriteString(value.ToString());
        }

        public Uri Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.TryGetScalarAsString(out var scalar) && scalar != null)
            {
                var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
                parser.Read();
                return uri;
            }
            throw new YamlSerializerException($"Cannot detect a scalar value of Uri : {parser.CurrentEventType} {parser.GetScalarAsString()}");
        }
    }
}
