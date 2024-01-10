#nullable enable
using System.Collections.Generic;
using System.Linq;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class InterfaceEnumerableFormatter<T> : IYamlFormatter<IEnumerable<T>?>
    {
        public void Serialize(ref Utf8YamlEmitter emitter, IEnumerable<T>? value, YamlSerializationContext context)
        {
            if (value is null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginSequence();
            if (value.Any())
            {
                var elementFormatter = context.Resolver.GetFormatterWithVerify<T>();
                foreach (var x in value)
                {
                    elementFormatter.Serialize(ref emitter, x, context);
                }
                emitter.EndSequence(true);
                return;
            }
            emitter.EndSequence(false);
        }

        public IEnumerable<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
            }

            List<T> list = new List<T>();
            parser.ReadWithVerify(ParseEventType.SequenceStart);

            var elementFormatter = context.Resolver.GetFormatterWithVerify<T>();
            while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
            {
                list.Add(context.DeserializeWithAlias(elementFormatter, ref parser));
            }
            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return list;
        }
    }
}
