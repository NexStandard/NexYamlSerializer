#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class KeyValuePairFormatter<TKey, TValue> : YamlSerializer<KeyValuePair<TKey, TValue>>
{
    protected override void Write(ISerializationWriter stream, KeyValuePair<TKey, TValue> value, DataStyle style)
    {
        stream.BeginSequence(style);
        stream.Write(value.Key);
        stream.Write(value.Value);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref KeyValuePair<TKey, TValue> value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var key = default(TKey);
        var val = default(TValue);
        context.DeserializeWithAlias(ref parser, ref key);
        context.DeserializeWithAlias(ref parser, ref val);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
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

