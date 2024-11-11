using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serialization.Formatters;

public class DictionaryInterfaceFormatter<TKey, TValue> : YamlSerializer<IDictionary<TKey, TValue>?>
    where TKey : notnull
{
    public override void Write(IYamlWriter stream, IDictionary<TKey, TValue>? value, DataStyle style)
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
                    kvp.Write(stream, x);
                }
            });
        }
    }

    public override void Read(IYamlReader parser, ref IDictionary<TKey, TValue>? value, ref ParseResult result)
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
            listFormatter.Read(parser, ref keyValuePairs, ref result);

            value = keyValuePairs?.ToDictionary() ?? [];
        }
    }
}

