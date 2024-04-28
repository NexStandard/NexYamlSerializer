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
        return DictionaryFormatterHelper.Deserialize<TKey,TValue>(ref parser, context);
    }

    public override void Serialize(ISerializationWriter stream, Dictionary<TKey, TValue>? value, DataStyle style = DataStyle.Normal)
    {
        DictionaryFormatterHelper.Serialize(stream, value!, style);
    }
}
internal class DictionaryFormatterHelper : IYamlFormatterHelper
{
    public static void Serialize<TKey,TValue>(ISerializationWriter stream, Dictionary<TKey, TValue> value, DataStyle style = DataStyle.Normal)
        where TKey : notnull
    {
        YamlSerializer<TKey> keyFormatter = null!;
        YamlSerializer<TValue> valueFormatter = null!;
        if (typeof(TKey).IsValueType || typeof(TKey) == typeof(string))
        {
            keyFormatter = stream.SerializeContext.Resolver.GetFormatter<TKey>();
        }
        if (typeof(TValue).IsValueType || typeof(TValue) == typeof(string))
        {
            valueFormatter = stream.SerializeContext.Resolver.GetFormatter<TValue>();
        }

        if (keyFormatter is null)
        {
            stream.BeginSequence(style);
            if (valueFormatter is null)
            {
                foreach (var x in value)
                {
                    stream.Write(x, style);
                }
            }
            else
            {
                foreach (var x in value)
                {
                    stream.BeginSequence(style);
                    stream.Write(x.Key);
                    stream.Write(x.Value);
                    stream.EndSequence();
                }
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
                    stream.Write(x.Value);
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
    public static Dictionary<TKey, TValue>? Deserialize<TKey, TValue>(ref YamlParser parser, YamlDeserializationContext context)
        where TKey : notnull
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
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.Register(this, typeof(Dictionary<,>), typeof(Dictionary<,>));
        resolver.RegisterGenericFormatter(typeof(Dictionary<,>), typeof(DictionaryFormatter<,>));
        resolver.RegisterFormatter(typeof(Dictionary<,>));

        resolver.Register(this, typeof(Dictionary<,>), typeof(IDictionary<,>));
        resolver.Register(this, typeof(Dictionary<,>), typeof(IReadOnlyDictionary<,>));

    }

    public YamlSerializer Instantiate(Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
        {
            var generatorType = typeof(DictionaryFormatter<,>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0], genericParams[1] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
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

