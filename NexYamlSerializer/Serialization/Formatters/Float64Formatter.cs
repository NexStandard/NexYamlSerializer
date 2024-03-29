#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization
{
    public class Float64Formatter : IYamlFormatter<double>
    {
        public static readonly Float64Formatter Instance = new();

        public void Serialize(ref Utf8YamlEmitter emitter, double value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            emitter.WriteDouble(value);
        }

        public double Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var result = parser.GetScalarAsDouble();
            parser.Read();
            return result;
        }
    }
}
