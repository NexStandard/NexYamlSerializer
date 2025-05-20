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

    public override async ValueTask<KeyValuePair<TKey, TValue>> Read(IYamlReader stream, ParseContext parseResult)
    {
        List<Task<KeyValuePair<TKey, TValue>>> tasks = new();
        stream.Move(ParseEventType.SequenceStart);

        var key = stream.Read<TKey>(new ParseContext());
        var value = stream.Read<TValue>(new ParseContext());

        stream.Move(ParseEventType.SequenceEnd);
        var k = await key;
        var v = await value;

        return new KeyValuePair<TKey, TValue>(k, v);
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

