#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System.Globalization;
using System;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt16Formatter : YamlSerializer<ushort>
{
    public static readonly UInt16Formatter Instance = new();

    protected override void Write(IYamlWriter stream, ushort value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ushort value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((ushort)result);
    }
}
