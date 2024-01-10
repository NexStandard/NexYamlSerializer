#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer;

namespace NexVYaml.Serialization
{
    public class InterfaceCollectionFormatter<T> : IYamlFormatter<ICollection<T>?>
    {
        public void Serialize(ref Utf8YamlEmitter emitter, ICollection<T>? value, YamlSerializationContext context)
        {
            // Unreachable as interfaces will never serialize something
        }


        public ICollection<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            parser.ReadWithVerify(ParseEventType.SequenceStart);

            var list = new List<T?>();
            var elementFormatter = context.Resolver.GetFormatterWithVerify<T>();
            while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
            {
                var value = context.DeserializeWithAlias(elementFormatter, ref parser);
                list.Add(value);
            }

            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return list!;
        }
    }
}
