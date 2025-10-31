using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

[CustomYamlSerializer(TargetType = typeof(Dictionary<,>))]
public class DictionarySerializer<TKey, TValue> : IYamlSerializer<Dictionary<TKey, TValue?>>
    where TKey : notnull
{
    public void Write<X>(WriteContext<X> context, Dictionary<TKey, TValue?> value, DataStyle style) where X : Node
    {
        if (value.Count == 0)
        {
            context.WriteEmptySequence("!Dictionary");
            return;
        }

        if (IsPrimitive(typeof(TKey)))
        {
            var resultContext = context.BeginMapping("!Dictionary", style);

            foreach (var x in value)
            {
                resultContext = resultContext.Write(x.Key.ToString() ?? "", x.Value, style);
            }
            resultContext.End(context);
        }
        else
        {
            var serializer = new ListSerializer<KeyValuePair<TKey, TValue?>>()
            {
                CustomTag = "!Dictionary"
            };
            serializer.Write(context, value.ToList(), style);
        }
    }

    public async ValueTask<Dictionary<TKey, TValue?>> Read(Scope scope, Dictionary<TKey, TValue?>? parseResult)
    {
        var map = parseResult ?? new();
        map.Clear();

        if (scope is MappingScope mapping && IsPrimitive(typeof(TKey)))
        {
            var tasks = new List<ValueTask<KeyValuePair<TKey, TValue?>>>();
            foreach (var kvp in mapping)
            {
                var key = ParsePrimitive<TKey>(kvp.Key);
                var value = kvp.Value.Read<TValue>();
                tasks.Add(ConvertToKeyValuePair(key, value));
            }
            foreach(var result in tasks)
            {
                var kvp = await result;
                map.Add(kvp.Key,kvp.Value);
            }
        }
        else
        {
            var listSerializer = new ListSerializer<KeyValuePair<TKey, TValue?>>();
            var kvp = await listSerializer.Read(scope, null);

            // can't be null as !!null wouldn't reach this serializer
            kvp.ForEach(x => map.Add(x.Key, x.Value));
        }
        return map;
    }
    private static T ParsePrimitive<T>(string key)
    {
        var type = typeof(T);

        if (type == typeof(string)) return (T)(object)key;
        if (type == typeof(bool)) return (T)(object)bool.Parse(key);
        if (type == typeof(byte)) return (T)(object)byte.Parse(key);
        if (type == typeof(sbyte)) return (T)(object)sbyte.Parse(key);
        if (type == typeof(char)) return (T)(object)char.Parse(key);
        if (type == typeof(short)) return (T)(object)short.Parse(key);
        if (type == typeof(ushort)) return (T)(object)ushort.Parse(key);
        if (type == typeof(int)) return (T)(object)int.Parse(key);
        if (type == typeof(uint)) return (T)(object)uint.Parse(key);
        if (type == typeof(long)) return (T)(object)long.Parse(key);
        if (type == typeof(ulong)) return (T)(object)ulong.Parse(key);
        if (type == typeof(float)) return (T)(object)float.Parse(key);
        if (type == typeof(double)) return (T)(object)double.Parse(key);
        if (type == typeof(decimal)) return (T)(object)decimal.Parse(key);
        if (type == typeof(DateTime)) return (T)(object)DateTime.Parse(key);
        if (type == typeof(TimeSpan)) return (T)(object)TimeSpan.Parse(key);

        throw new NotSupportedException($"Unsupported primitive type: {type.Name}");
    }
    private static async ValueTask<KeyValuePair<TKey, TValue?>> ConvertToKeyValuePair(TKey key, ValueTask<TValue?> value)
    {
        var k = key;
        var v = await value;
        return new KeyValuePair<TKey, TValue?>(k, v);
    }
    private static bool IsPrimitive(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(decimal) ||
               type == typeof(string) ||
               type == typeof(DateTime) ||
               type == typeof(TimeSpan);
    }
}

internal class DictionarySerializerFactory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.Register(this, typeof(Dictionary<,>), typeof(Dictionary<,>));
        resolver.RegisterGenericSerializer(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
        resolver.RegisterSerializer(typeof(Dictionary<,>));
        resolver.RegisterTag("!Dictionary", typeof(Dictionary<,>));
        resolver.Register(this, typeof(Dictionary<,>), typeof(IDictionary<,>));
        resolver.Register(this, typeof(Dictionary<,>), typeof(IReadOnlyDictionary<,>));
    }

    public IYamlSerializer Instantiate(Type type)
    {
        var generatorType = typeof(DictionarySerializer<,>);
        var genericParams = type.GenericTypeArguments;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        Type filledGeneratorType;
        if (genericTypeDefinition == typeof(IDictionary<,>)
            || genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
        {
            filledGeneratorType = generatorType.MakeGenericType(genericParams[0], genericParams[1]);
        }
        else
        {
            filledGeneratorType = generatorType.MakeGenericType(genericParams);
        }

        return (IYamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
    }
}
