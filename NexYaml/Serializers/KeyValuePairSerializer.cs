using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class KeyValuePairSerializer<TKey, TValue> : YamlSerializer<KeyValuePair<TKey?, TValue?>>
{
    public override void Write<X>(WriteContext<X> context, KeyValuePair<TKey?, TValue?> value, DataStyle style)
    {
        context.BeginSequence("!KeyValue", style)
            .Write(value.Key, style)
            .Write(value.Value, style)
            .End(context);
    }

    public override async ValueTask<KeyValuePair<TKey?, TValue?>> Read(Scope scope, KeyValuePair<TKey?, TValue?> parseResult)
    {
        List<Task<KeyValuePair<TKey, TValue>>> tasks = new();
        var scalarScope = scope.As<SequenceScope>().ToList();
        var k = await scalarScope[0].Read<TKey?>(default);
        var v = await scalarScope[1].Read<TValue?>(default);
        return new KeyValuePair<TKey?, TValue?>(k, v);
    }
}
file sealed class KeyValuePairSerializerFactory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.Register(this, typeof(KeyValuePair<,>), typeof(KeyValuePair<,>));
        resolver.RegisterGenericSerializer(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));
        resolver.RegisterSerializer(typeof(KeyValuePairSerializer<,>));
    }

    public YamlSerializer Instantiate(Type target)
    {
        var gen = typeof(KeyValuePairSerializer<,>);
        var genParams = target.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

