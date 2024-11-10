using NexYaml.Serialization;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexYaml.Serialization.Formatters;

public class KeyValuePairFormatter<TKey, TValue> : YamlSerializer<KeyValuePair<TKey, TValue>>
{
    public override void Write(IYamlWriter stream, KeyValuePair<TKey, TValue> value, DataStyle style)
    {
        stream.WriteSequence(style, () =>
        {
            stream.Write(value.Key, style);
            stream.Write(value.Value, style);
        });
    }

    public override void Read(IYamlReader stream, ref KeyValuePair<TKey, TValue> value)
    {
        var key = default(TKey);
        var val = default(TValue);
        stream.ReadSequence(() =>
        {
            stream.Read(ref key);
            stream.Read(ref val);
        });
        value = new KeyValuePair<TKey, TValue>(key!, val!);
    }
}
file sealed class KeyValuePairFormatterHelper : IYamlFormatterHelper
{
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.Register(this, typeof(KeyValuePair<,>), typeof(KeyValuePair<,>));
        resolver.RegisterGenericFormatter(typeof(KeyValuePair<,>), typeof(KeyValuePairFormatter<,>));
        resolver.RegisterFormatter(typeof(KeyValuePairFormatter<,>));
    }

    public YamlSerializer Instantiate(Type target)
    {
        var gen = typeof(KeyValuePairFormatter<,>);
        var genParams = target.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

