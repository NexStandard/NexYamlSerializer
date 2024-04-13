using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexYamlSerializer.Serialization.Formatters;
public class EmptyFormatter<T> : YamlSerializer<T>,IYamlFormatter<T>
{
    public static IYamlFormatter<T> Empty() => new EmptyFormatter<T>();
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
