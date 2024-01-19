#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization
{
    public class NullableFormatter<T> : IYamlFormatter<T?> where T : struct
    {
        IYamlFormatter<T> yamlFormatter;
        public NullableFormatter(IYamlFormatter<T> formatter) { yamlFormatter = formatter; }

        public void Serialize(ref Utf8YamlEmitter emitter, T? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            if (value is null)
            {
                emitter.WriteNull();
            }
            else
            {
                yamlFormatter.Serialize(ref emitter, value.Value, context);
            }
        }

        public T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return null;
            }

            return new T?(yamlFormatter.Deserialize(ref parser,context));
        }
    }

    public sealed class StaticNullableFormatter<T> : IYamlFormatter<T?> where T : struct
    {
        readonly IYamlFormatter<T> underlyingFormatter;

        public StaticNullableFormatter(IYamlFormatter<T> underlyingFormatter)
        {
            this.underlyingFormatter = underlyingFormatter;
        }

        public void Serialize(ref Utf8YamlEmitter emitter, T? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            if (value.HasValue)
            {
                underlyingFormatter.Serialize(ref emitter, value.Value, context);
            }
            else
            {
                emitter.WriteNull();
            }
        }

        public T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return null;
            }
            return underlyingFormatter.Deserialize(ref parser, context);
        }
    }
}
