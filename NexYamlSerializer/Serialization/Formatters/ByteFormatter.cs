#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;

namespace NexVYaml.Serialization;

public class ByteFormatter : YamlSerializer<byte>
{
    public static readonly ByteFormatter Instance = new();

    protected override void Write(IYamlWriter stream, byte value, DataStyle style)
    {
        Span<byte> span = stackalloc byte[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Serialize(span[..written]);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref byte value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((byte)result);
    }
}
