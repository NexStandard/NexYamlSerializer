using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DictionarySerializer<TKey, TValue> : YamlSerializer<Dictionary<TKey, TValue>?>
    where TKey : notnull
{
    public override void Write(IYamlWriter stream, Dictionary<TKey, TValue>? value, DataStyle style = DataStyle.Normal)
    {
        stream.IsRedirected = false;
        DictionarySerializerFactory.Serialize(stream, value!, style);
    }

    public override void Read(IYamlReader stream, ref Dictionary<TKey, TValue>? value, ref ParseResult parseResult)
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
        }
        else
        {
            var listSerializer = new ListSerializer<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = default(List<KeyValuePair<TKey, TValue>>);
            listSerializer.Read(stream, ref keyValuePairs, ref parseResult);

            value = keyValuePairs?.ToDictionary() ?? [];
        }
    }
}
internal class DictionarySerializerFactory : IYamlSerializerFactory
{
    public static void Serialize<TKey, TValue>(IYamlWriter stream, Dictionary<TKey, TValue> value, DataStyle style = DataStyle.Normal)
        where TKey : notnull
    {
        if (SerializerExtensions.IsPrimitive(typeof(TKey)))
        {
            stream.BeginMapping(style);
            foreach (var x in value)
            {
                stream.Write(x.Key, style);
                stream.Write(x.Value, style);
            }
            stream.EndMapping();
            return;
        }
        else
        {
            var kvp = new KeyValuePairSerializer<TKey, TValue>();
            using (stream.SequenceScope(style))
            {
                foreach (var x in value)
                {
                    kvp.Write(stream, x);
                }
            }
        }
    }

    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.Register(this, typeof(Dictionary<,>), typeof(Dictionary<,>));
        resolver.RegisterGenericSerializer(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
        resolver.RegisterSerializer(typeof(Dictionary<,>));

        resolver.Register(this, typeof(Dictionary<,>), typeof(IDictionary<,>));
        resolver.Register(this, typeof(Dictionary<,>), typeof(IReadOnlyDictionary<,>));

    }

    public YamlSerializer Instantiate(Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
        {
            var generatorType = typeof(DictionarySerializer<,>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0], genericParams[1] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
        {
            var generatorType = typeof(DictionarySerializer<,>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0], genericParams[1] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        var gen = typeof(DictionarySerializer<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

