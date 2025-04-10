using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DictionaryInterfaceSerializer<TKey, TValue> : YamlSerializer<IDictionary<TKey, TValue>?>
    where TKey : notnull
{
    public override void Write(IYamlWriter stream, IDictionary<TKey, TValue>? value, DataStyle style)
    {
        if (SerializerExtensions.IsPrimitive(typeof(TKey)))
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
            var kvp = new KeyValuePairSerializer<TKey, TValue>();
            stream.WriteSequence(style, () =>
            {
                foreach (var x in value)
                {
                    kvp.Write(stream, x);
                }
            });
        }
    }

    public override void Read(IYamlReader stream, ref IDictionary<TKey, TValue>? value, ref ParseResult result)
    {

        var map = new Dictionary<TKey, TValue>();
        if (SerializerExtensions.IsPrimitive(typeof(TKey)))
        {
            stream.Move(ParseEventType.MappingStart);

            while (stream.HasKeyMapping)
            {
                var key = default(TKey);
                stream.Read(ref key);
                var val = default(TValue);
                stream.Read(ref val);
                map.Add(key!, val!);
            }

            stream.Move(ParseEventType.MappingEnd);
            value = map;
            return;
        }
        else
        {
            var listSerializer = new ListSerializer<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = default(List<KeyValuePair<TKey, TValue>>);
            listSerializer.Read(stream, ref keyValuePairs, ref result);

            value = keyValuePairs?.ToDictionary() ?? [];
        }
    }
}

