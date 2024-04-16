#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class DictionaryFormatter<TKey, TValue> : YamlSerializer<Dictionary<TKey, TValue>?>
    where TKey : notnull
{
    public override Dictionary<TKey, TValue>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
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

    public override void Serialize(ref ISerializationWriter stream, Dictionary<TKey, TValue> value, DataStyle style = DataStyle.Normal)
    {
        YamlSerializer<TKey> keyFormatter = null!;
        YamlSerializer<TValue> valueFormatter = null!;
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            keyFormatter = stream.SerializeContext.Resolver.GetFormatter<TKey>();
        }
        if (FormatterExtensions.IsPrimitive(typeof(TValue)))
        {
            valueFormatter = stream.SerializeContext.Resolver.GetFormatter<TValue>();
        }

        if (keyFormatter is null)
        {
            stream.BeginSequence(style);

            foreach (var x in value)
            {
                stream.Serialize(x, style);
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
                    stream.Serialize(x.Value);
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
file class DictionaryFormatterHelper : IYamlFormatterHelper
{
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.Register(this, typeof(Dictionary<,>), typeof(Dictionary<,>));
        resolver.RegisterGenericFormatter(typeof(Dictionary<,>), typeof(DictionaryFormatter<,>));
        resolver.RegisterFormatter(typeof(Dictionary<,>));

        resolver.Register(this, typeof(Dictionary<,>), typeof(System.Collections.Generic.IDictionary<,>));
        resolver.Register(this, typeof(Dictionary<,>), typeof(System.Collections.Generic.IReadOnlyDictionary<,>));

    }

    public YamlSerializer Instantiate(Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
        {
            var generatorType = typeof(DictionaryFormatter<,>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0], genericParams[1] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyDictionary<,>))
        {
            var generatorType = typeof(DictionaryFormatter<,>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0], genericParams[1] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        var gen = typeof(DictionaryFormatter<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

