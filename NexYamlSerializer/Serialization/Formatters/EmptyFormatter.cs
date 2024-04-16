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
    public override T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        return default!;
    }

    public override void Serialize(ref ISerializationWriter stream, T value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(YamlCodes.Null0);
    }
}
