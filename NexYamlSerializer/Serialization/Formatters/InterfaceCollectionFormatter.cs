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
            if (value is null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginSequence();
            if (value.Count > 0)
            {
                foreach (var x in value)
                {
                    context.Serialize(ref emitter, x);
                }
            }
            emitter.EndSequence(value.Count == 0);
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
            while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
            {
                var value = context.DeserializeWithAlias<T>(ref parser);
                list.Add(value);
            }

            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return list!;
        }
    }
}
