#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class DictionaryInterfaceFormatter<TKey, TValue> : YamlSerializer<IDictionary<TKey, TValue>?>
    where TKey : notnull
{
    protected override void Write(IYamlWriter stream, IDictionary<TKey, TValue>? value, DataStyle style)
    {
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            stream.BeginMapping(style);
            {
                foreach (var x in value!)
                {
                    stream.Write(x.Key, style);
                    stream.Write(x.Value, style);
                }
            }
            stream.EndMapping();
            return;
        }
        else
        {
            var kvp = new KeyValuePairFormatter<TKey, TValue>();
            stream.BeginSequence(style);
            foreach (var x in value!)
            {
                kvp.Serialize(stream, x);
            }
            stream.EndSequence();
        }
    }

    protected override void Read(YamlParser parser, ref IDictionary<TKey, TValue>? value)
    {

        var map = new Dictionary<TKey, TValue>();
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            var keyFormatter = parser.Resolver.GetFormatter<TKey>();
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (parser.HasKeyMapping)
            {
                var key = default(TKey);
                parser.DeserializeWithAlias(keyFormatter, ref parser, ref key);
                var val = default(TValue);
                parser.DeserializeWithAlias(ref parser, ref val);
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
            parser.DeserializeWithAlias(listFormatter, ref parser, ref keyValuePairs);

            value = keyValuePairs?.ToDictionary() ?? [];
        }
    }
}

