#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization
{
    public class ArrayFormatter<T> : IYamlFormatter<T[]?>
    {
        public void Serialize(ref Utf8YamlEmitter emitter, T[]? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
        {
            if (value is null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginSequence();
            foreach (var x in value)
            {
                context.Serialize(ref emitter, x);
            }
            emitter.EndSequence(value.Length == 0);
        }

        public T[]? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            parser.ReadWithVerify(ParseEventType.SequenceStart);

            var list = new List<T>();
            while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
            {
                var value = context.DeserializeWithAlias<T>(ref parser);
                list.Add(value);
            }

            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return list.ToArray();
        }
    }
}
