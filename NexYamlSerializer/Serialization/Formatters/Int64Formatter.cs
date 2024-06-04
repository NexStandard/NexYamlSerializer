#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int64Formatter : YamlSerializer<long>
{
    public static readonly Int64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, long value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref long value)
    {
        var result = parser.GetScalarAsInt64();
        parser.Read();
        value = result;
    }
}
