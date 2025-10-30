using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using Stride.Core.Extensions;

namespace NexYaml.Serializers;

[CustomYamlSerializer(TargetType = typeof(Dictionary<,>))]
public class DictionarySerializer<TKey, TValue> : YamlSerializer<Dictionary<TKey, TValue?>>
    where TKey : notnull
{
    public override void Write<X>(WriteContext<X> context, Dictionary<TKey, TValue?> value, DataStyle style)
    {
        if (IsPrimitive(typeof(TKey)))
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
                    resultContext = resultContext.Write(x.Key.ToString()!, x.Value, style);
                }
                resultContext.End(context);
            }
        }
        else
        {
            if (value.Count == 0)
            {
                context.WriteEmptySequence("!Dictionary");
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
    }

    public override async ValueTask<Dictionary<TKey, TValue?>?> Read(Scope scope, ParseContext parseResult)
    {
        var map = parseResult.DataMemberMode is DataMemberMode.Content ? (Dictionary<TKey, TValue?>)parseResult.Value! : [];
        if (scope is MappingScope mapping && DictionarySerializer<TKey, TValue>.IsPrimitive(typeof(TKey)))
        {
            List<ValueTask<KeyValuePair<TKey, TValue?>>> tasks = new();
            foreach (var kvp in mapping)
            {
                var key = ParsePrimitive<TKey>(kvp.Key);
                var value = kvp.Value.Read<TValue>(new ParseContext());
                tasks.Add(DictionarySerializer<TKey, TValue>.ConvertToKeyValuePair(key!, value));
            }
            foreach(var result in tasks)
            {
                var kvp = await result;
                map.Add(kvp.Key,kvp.Value);
            }
            return map;
        }
        else
        {
            var listSerializer = new ListSerializer<KeyValuePair<TKey, TValue?>>();
            var kvp = await listSerializer.Read(scope, new ParseContext());

            // can't be null as !!null wouldnt reach this serializer
            kvp!.ForEach(x => map.Add(x.Key!, x.Value));
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
               type == typeof(bool) ||
               type == typeof(byte) ||
               type == typeof(sbyte) ||
               type == typeof(char) ||
               type == typeof(short) ||
               type == typeof(ushort) ||
               type == typeof(int) ||
               type == typeof(uint) ||
               type == typeof(long) ||
               type == typeof(ulong) ||
               type == typeof(float) ||
               type == typeof(double) ||
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
