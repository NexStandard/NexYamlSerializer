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
    protected override void Write(ISerializationWriter stream, Dictionary<TKey, TValue>? value, DataStyle style = DataStyle.Normal)
    {
        DictionaryFormatterHelper.Serialize(stream, value!, style);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Dictionary<TKey, TValue>? value)
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
                context.DeserializeWithAlias(ref parser, ref val);
                map.Add(key, val!);
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            value = map;
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
internal class DictionaryFormatterHelper : IYamlFormatterHelper
{
    public static void Serialize<TKey,TValue>(ISerializationWriter stream, Dictionary<TKey, TValue> value, DataStyle style = DataStyle.Normal)
        where TKey : notnull
    {
        if (FormatterExtensions.IsPrimitive(typeof(TKey)))
        {
            stream.BeginMapping(style);
            {
                foreach (var x in value)
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
            foreach (var x in value)
            {
                kvp.Serialize(stream, x);
            }
            stream.EndSequence();
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

