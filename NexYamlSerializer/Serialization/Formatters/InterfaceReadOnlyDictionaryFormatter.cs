#nullable enable
using System.Collections.Generic;
using System.Linq;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;

namespace NexVYaml.Serialization;

public class InterfaceReadOnlyDictionaryFormatter<TKey, TValue> :YamlSerializer<IReadOnlyDictionary<TKey,TValue>>, IYamlFormatter<IReadOnlyDictionary<TKey, TValue>?>
    where TKey : notnull
{
    public override IReadOnlyDictionary<TKey, TValue>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return default;
        }
        var map = new Dictionary<TKey, TValue>();
        if (this.IsPrimitiveType(typeof(TKey)))
        {
            var keyFormatter = context.Resolver.GetFormatter<TKey>();
            parser.ReadWithVerify(ParseEventType.MappingStart);


            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = context.DeserializeWithAlias(keyFormatter, ref parser);
                var value = context.DeserializeWithAlias<TValue>(ref parser);
                map.Add(key, value);
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return map;
        }
        else
        {
            var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = context.DeserializeWithAlias(listFormatter, ref parser);

            return keyValuePairs?.ToDictionary() ?? [];
        }
    }

    public override void Serialize(ref IYamlStream stream, IReadOnlyDictionary<TKey, TValue> value, DataStyle style = DataStyle.Normal)
    {
        stream.SerializeContext.IsRedirected = false;

        YamlSerializer<TKey> keyFormatter = null;
        YamlSerializer<TValue> valueFormatter = null;
        if (this.IsPrimitiveType(typeof(TKey)))
        {
            keyFormatter = NewSerializerRegistry.Instance.GetFormatter<TKey>();
        }
        if (this.IsPrimitiveType(typeof(TValue)))
            valueFormatter = NewSerializerRegistry.Instance.GetFormatter<TValue>();

        if (keyFormatter == null)
        {
            stream.Emitter.BeginSequence();
            if (value.Count > 0)
            {
                var elementFormatter = new KeyValuePairFormatter<TKey, TValue>();
                foreach (var x in value)
                {
                    elementFormatter.Serialize(ref stream, x);
                }
            }
            stream.Emitter.EndSequence();
        }
        else if (valueFormatter == null)
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    stream.Write(x.Value);
                }
            }
            stream.Emitter.EndMapping();
        }
        else
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    valueFormatter.Serialize(ref stream, x.Value, style);
                }
            }
            stream.Emitter.EndMapping();
        }
    }
}

