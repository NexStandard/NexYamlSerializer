using System.IO;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DictionarySerializer<TKey, TValue> : YamlSerializer<Dictionary<TKey, TValue>?>
    where TKey : notnull
{
    public override void Write<X>(WriteContext<X> context, Dictionary<TKey, TValue>? value, DataStyle style)
    {
        if (SerializerExtensions.IsPrimitive(typeof(TKey)))
        {
            if (value!.Count == 0)
            {
                context.WriteEmptyMapping("!Dictionary");
            }
            else
            {
                var resultContext = context.BeginMapping("!Dictionary", style);

                foreach (var x in value)
                {
                    resultContext = resultContext.Write(x.Key.ToString(), x.Value,style);
                }
                resultContext.End(context);
            }
        }
        else
        {
            if (value?.Count == 0)
            {
                context.WriteEmptySequence("!Dictionary");
            }
            else
            {
                var serializer = new ListSerializer<KeyValuePair<TKey, TValue>>()
                {
                    CustomTag = "!Dictionary"
                };
                serializer.Write(context, value.ToList(), style);
            }
        }
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
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.Register(this, typeof(Dictionary<,>), typeof(Dictionary<,>));
        resolver.RegisterGenericSerializer(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
        resolver.RegisterSerializer(typeof(Dictionary<,>));
        resolver.RegisterTag("Dictionary", typeof(Dictionary<,>));
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