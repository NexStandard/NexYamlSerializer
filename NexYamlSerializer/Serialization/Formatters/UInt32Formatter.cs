#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System.Globalization;
using System;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt32Formatter : YamlSerializer<uint>
{
    public static readonly UInt32Formatter Instance = new();

    protected override void Write(IYamlWriter stream, uint value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref uint value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = result;
    }
}
