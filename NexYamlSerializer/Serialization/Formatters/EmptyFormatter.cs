using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;

namespace NexYamlSerializer.Serialization.Formatters;
internal class EmptyFormatter<T> : IYamlFormatter<T>
{
    public static IYamlFormatter<T> Empty() => new EmptyFormatter<T>();
    public T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        return default!;
    }

    public void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
    {
        emitter.WriteNull();
    }
}
