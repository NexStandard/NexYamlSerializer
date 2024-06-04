#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Float64Formatter : YamlSerializer<double>
{
    public static readonly Float64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, double value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref double value)
    {
        var result = parser.GetScalarAsDouble();
        parser.Read();
        value = result;
    }
}
