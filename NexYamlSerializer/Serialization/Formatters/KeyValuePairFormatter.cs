#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{
    public class KeyValuePairFormatter<TKey, TValue> : IYamlFormatter<KeyValuePair<TKey, TValue>>
    {
        public void Serialize(ref Utf8YamlEmitter emitter, KeyValuePair<TKey, TValue> value, YamlSerializationContext context)
        {
            emitter.BeginSequence();
            context.Serialize(ref emitter, value.Key);
            context.Serialize(ref emitter, value.Value);
            emitter.EndSequence(false);
        }

        public KeyValuePair<TKey, TValue> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                return default;
            }

            parser.ReadWithVerify(ParseEventType.SequenceStart);
            var key = context.DeserializeWithAlias<TKey>(ref parser);
            var value = context.DeserializeWithAlias<TValue>(ref parser);
            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}

