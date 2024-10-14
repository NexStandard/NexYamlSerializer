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

    protected override void Read(IYamlReader parser, ref IDictionary<TKey, TValue>? value)
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
                parser.Read(ref val);
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

