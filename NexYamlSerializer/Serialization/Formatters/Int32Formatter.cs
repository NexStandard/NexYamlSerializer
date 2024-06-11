#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;

namespace NexVYaml.Serialization;

public class Int32Formatter : YamlSerializer<int>
{
    public static readonly Int32Formatter Instance = new();

    protected override void Write(IYamlWriter stream, int value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref int value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = result;
    }
}
