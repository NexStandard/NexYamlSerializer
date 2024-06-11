#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;

namespace NexVYaml.Serialization;

public class Float32Formatter : YamlSerializer<float>
{
    public static readonly Float32Formatter Instance = new();

    protected override void Write(IYamlWriter stream, float value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref float value)
    {
        var result = parser.GetScalarAsFloat();
        parser.Read();
        value = result;
    }
}
