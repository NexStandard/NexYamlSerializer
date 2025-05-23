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

    public override async ValueTask<Dictionary<TKey, TValue?>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        var map = parseResult.DataMemberMode is DataMemberMode.Content ? (Dictionary<TKey, TValue?>)parseResult.Value! : [];
        if (IsPrimitive(typeof(TKey)))
        {
            List<Task<KeyValuePair<TKey, TValue?>>> tasks = new();
            stream.Move(ParseEventType.MappingStart);

            while (stream.HasKeyMapping)
            {
                var key = stream.Read<TKey>(new ParseContext());
                var value = stream.Read<TValue>(new ParseContext());
                tasks.Add(ConvertToKeyValuePair(key!, value));
            }
            stream.Move(ParseEventType.MappingEnd);
            (await Task.WhenAll(tasks)).ForEach(x => map.Add(x.Key, x.Value));
            return map;
        }
        else
        {
            var listSerializer = new ListSerializer<KeyValuePair<TKey, TValue?>>();
            var kvp = await listSerializer.Read(stream, new ParseContext());

            // can't be null as !!null wouldnt reach this serializer
            kvp!.ForEach(x => map.Add(x.Key!, x.Value));
        }
        return map;
    }

    private static async Task<KeyValuePair<TKey, TValue?>> ConvertToKeyValuePair(ValueTask<TKey> key, ValueTask<TValue?> value)
    {
        var k = await key;
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
