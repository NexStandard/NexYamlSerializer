#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System.Globalization;
using System;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt64Formatter : YamlSerializer<ulong>
{
    public static readonly UInt64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, ulong value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[20];

        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ulong value)
    {
        var result = parser.GetScalarAsUInt64();
        parser.Read();
        value = result;
    }
}
