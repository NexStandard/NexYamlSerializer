#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;

namespace NexVYaml.Serialization;

public class Int64Formatter : YamlSerializer<long>
{
    public static readonly Int64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, long value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref long value)
    {
        var result = parser.GetScalarAsInt64();
        parser.Read();
        value = result;
    }
}
