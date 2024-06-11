#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;

namespace NexVYaml.Serialization;

public class Float64Formatter : YamlSerializer<double>
{
    public static readonly Float64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, double value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref double value)
    {
        var result = parser.GetScalarAsDouble();
        parser.Read();
        value = result;
    }
}
