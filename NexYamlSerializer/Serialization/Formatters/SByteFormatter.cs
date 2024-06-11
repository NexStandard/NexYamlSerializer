#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System;
using System.Globalization;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class SByteFormatter : YamlSerializer<sbyte>
{
    public static readonly SByteFormatter Instance = new();

    protected override void Write(IYamlWriter stream, sbyte value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref sbyte value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = checked((sbyte)result);
    }
}
