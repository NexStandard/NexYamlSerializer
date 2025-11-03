using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class KeyValuePairSerializer<TKey, TValue> : IYamlSerializer<KeyValuePair<TKey?, TValue?>>
{
    public void Write<X>(WriteContext<X> context, KeyValuePair<TKey?, TValue?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!KeyValue", style)
            .Write(value.Key, style)
            .Write(value.Value, style)
            .End(context);
    }

    public async ValueTask<KeyValuePair<TKey?, TValue?>> Read(Scope scope, KeyValuePair<TKey?, TValue?> parseResult)
    {
        ValueTask<TKey?> kTask = default;
        ValueTask<TValue?> vTask = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                kTask = subscope.Read<TKey?>();
            else if (i == 1)
                vTask = subscope.Read<TValue?>();
            i++;
        }

        return new KeyValuePair<TKey?, TValue?>(await kTask, await vTask);
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

    public IYamlSerializer Instantiate(Type target)
    {
        var gen = typeof(KeyValuePairSerializer<,>);
        var genParams = target.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
