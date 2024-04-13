#nullable enable
using System;
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class KeyValuePairFormatter<TKey, TValue> : YamlSerializer<KeyValuePair<TKey,TValue>>,IYamlFormatter<KeyValuePair<TKey, TValue>>
{
    public override KeyValuePair<TKey, TValue> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            return default;
        }

        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var key = context.DeserializeWithAlias<TKey>(ref parser);
        var value = context.DeserializeWithAlias<TValue>(ref parser);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return new KeyValuePair<TKey, TValue>(key, value);
    }

    public override void Serialize(ref ISerializationWriter stream, KeyValuePair<TKey, TValue> value, DataStyle style = DataStyle.Normal)
    {
        stream.Emitter.BeginSequence();
        stream.Serialize(value.Key);
        stream.Serialize(value.Value);
        stream.Emitter.EndSequence();
    }
}
file class NexSourceGenerated_NexYamlTest_ComplexCasesTempListHelper : IYamlFormatterHelper
{
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.Register(this, typeof(KeyValuePair<,>), typeof(KeyValuePair<,>));
        resolver.RegisterGenericFormatter(typeof(KeyValuePair<,>), typeof(KeyValuePairFormatter<,>));
        resolver.RegisterFormatter(typeof(KeyValuePairFormatter<,>));
    }
    public IYamlFormatter Create(Type type)
    {

        var gen = typeof(KeyValuePairFormatter<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlFormatter)Activator.CreateInstance(fillGen);
    }

    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(KeyValuePairFormatter<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen);
    }
}

