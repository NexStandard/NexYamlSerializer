#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Float64Formatter : YamlSerializer<double>,IYamlFormatter<double>
{
    public static readonly Float64Formatter Instance = new();

    public override double Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsDouble();
        parser.Read();
        return result;
    }

    public override void Serialize(ref ISerializationWriter stream, double value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
