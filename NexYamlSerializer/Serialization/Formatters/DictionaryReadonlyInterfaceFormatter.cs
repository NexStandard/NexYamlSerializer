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
    protected override void Write(IYamlWriter stream, IReadOnlyDictionary<TKey, TValue> value, DataStyle style)
    {
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            stream.WriteMapping(style, () =>
            {
                foreach (var x in value)
                {
                    stream.Write(x.Key, style);
                    stream.Write(x.Value, style);
                }
            });
            return;
        }
        else
        {
            var kvp = new KeyValuePairFormatter<TKey, TValue>();
            stream.WriteSequence(style, () =>
            {
                foreach (var x in value)
                {
                    kvp.Serialize(stream, x);
                }
            });
        }
    }

    protected override void Read(IYamlReader parser,  ref IReadOnlyDictionary<TKey, TValue> value)
    {
        var map = new Dictionary<TKey, TValue>();
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (parser.HasKeyMapping)
            {
                var key = default(TKey);
                parser.Read(ref key);
                var val = default(TValue);
                parser.Read(ref value);
                map.Add(key!, val!);
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            value = map;
            return;
        }
        else
        {
            var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = default(List<KeyValuePair<TKey, TValue>>);
            listFormatter.Deserialize(parser, ref keyValuePairs);

            value = keyValuePairs?.ToDictionary() ?? [];
        }
    }
}

