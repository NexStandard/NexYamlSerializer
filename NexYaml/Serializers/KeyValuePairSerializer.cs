using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class KeyValuePairSerializer<TKey, TValue> : YamlSerializer<KeyValuePair<TKey, TValue>>
{
    public override void Write<X>(WriteContext<X> context, KeyValuePair<TKey, TValue> value, DataStyle style)
    {
        context.BeginSequence("!KeyValue", style)
            .Write(value.Key, style)
            .Write(value.Value, style)
            .End(context);
    }

    public override void Read(IYamlReader stream, ref KeyValuePair<TKey, TValue> value, ref ParseResult result)
    {
        var key = default(TKey);
        var val = default(TValue);
        using (stream.SequenceScope())
        {
            stream.Read(ref key);
            stream.Read(ref val);
        }
        value = new KeyValuePair<TKey, TValue>(key!, val!);
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

