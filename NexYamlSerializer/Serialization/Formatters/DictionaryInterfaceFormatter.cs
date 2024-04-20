#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class DictionaryInterfaceFormatter<TKey, TValue> : YamlSerializer<IDictionary<TKey, TValue>?>
{
    public override IDictionary<TKey, TValue>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return default;
        }
        var map = new Dictionary<TKey, TValue>();
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            var keyFormatter = context.Resolver.GetFormatter<TKey>();
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = default(TKey);
                context.DeserializeWithAlias(keyFormatter, ref parser, ref key);
                var value = default(TValue);
                context.DeserializeWithAlias(ref parser, ref value);
                map.Add(key, value!);
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return map;
        }
        else
        {
            var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = default(List<KeyValuePair<TKey, TValue>>);
            context.DeserializeWithAlias(listFormatter, ref parser, ref keyValuePairs);

            return keyValuePairs?.ToDictionary() ?? [];
        }
    }

    public override void Serialize(ISerializationWriter stream, IDictionary<TKey, TValue> value, DataStyle style = DataStyle.Normal)
    {

        YamlSerializer<TKey>? keyFormatter = null;
        YamlSerializer<TValue>? valueFormatter = null;
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            keyFormatter = stream.SerializeContext.Resolver.GetFormatter<TKey>();
        }
        if (FormatterExtensions.IsPrimitive(typeof(TValue)))
            valueFormatter = stream.SerializeContext.Resolver.GetFormatter<TValue>();

        if (keyFormatter == null)
        {
            stream.BeginSequence(style);
            foreach (var x in value)
            {
                stream.Write(x, style);
            }
            stream.EndSequence();
        }
        else if (valueFormatter == null)
        {
            stream.BeginMapping(style);
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    stream.Write(x.Value, style);
                }
            }
            stream.EndMapping();
        }
        else
        {
            stream.BeginMapping(style);
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    valueFormatter.Serialize(ref stream, x.Value, style);
                }
            }
            stream.EndMapping();
        }
    }
}

