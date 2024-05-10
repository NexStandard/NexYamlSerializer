#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class DictionaryReadonlyInterfaceFormatter<TKey, TValue> : YamlSerializer<IReadOnlyDictionary<TKey, TValue>>
    where TKey : notnull
{
    protected override void Write(SerializationWriter stream, IReadOnlyDictionary<TKey, TValue> value, DataStyle style)
    {

        YamlSerializer<TKey>? keyFormatter = null;
        YamlSerializer<TValue>? valueFormatter = null;
        
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            keyFormatter = NexYamlSerializerRegistry.Instance.GetFormatter<TKey>();
        }

        if (FormatterExtensions.IsPrimitive(typeof(TValue)))
        {
            valueFormatter = NexYamlSerializerRegistry.Instance.GetFormatter<TValue>();
        }

        if (keyFormatter == null)
        {
            stream.BeginSequence(style);

            var elementFormatter = new KeyValuePairFormatter<TKey, TValue>();
            foreach (var x in value)
            {
                elementFormatter.Serialize(stream, x);
            }

            stream.EndSequence();
        }
        else if (valueFormatter == null)
        {
            stream.BeginMapping(style);

            foreach (var x in value)
            {
                keyFormatter.Serialize(stream, x.Key, style);
                stream.Write(x.Value);
            }

            stream.EndMapping();
        }
        else
        {
            stream.BeginMapping(style);

            foreach (var x in value)
            {
                keyFormatter.Serialize(stream, x.Key, style);
                valueFormatter.Serialize(stream, x.Value, style);
            }

            stream.EndMapping();
        }
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref IReadOnlyDictionary<TKey, TValue> value)
    {
        var map = new Dictionary<TKey, TValue>();
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            var keyFormatter = context.Resolver.GetFormatter<TKey>();
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = default(TKey);
                context.DeserializeWithAlias(keyFormatter, ref parser, ref key);
                var val = default(TValue);
                context.DeserializeWithAlias(ref parser, ref value);
                map.Add(key, val!);
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            value = map;
            return;
        }
        else
        {
            var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = default(List<KeyValuePair<TKey, TValue>>);
            context.DeserializeWithAlias(listFormatter, ref parser, ref keyValuePairs);

            value = keyValuePairs?.ToDictionary() ?? [];
        }
    }
}

