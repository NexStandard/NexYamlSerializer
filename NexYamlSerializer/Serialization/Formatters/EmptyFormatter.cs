using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Linq;

namespace NexYamlSerializer.Serialization.Formatters;
public class EmptyFormatter<T> : YamlSerializer<T>
{
    public static YamlSerializer<T> EmptyS() => new EmptyFormatter<T>();

    protected override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        ReadOnlySpan<byte> bytes = YamlCodes.Null0;
        stream.Serialize(bytes);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref T value)
    {
        value = default!;
    }
}
