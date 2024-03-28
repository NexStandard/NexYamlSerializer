#nullable enable
using System;
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class KeyValuePairFormatter<TKey, TValue> : IYamlFormatter<KeyValuePair<TKey, TValue>>
{
    public void Serialize(ref Utf8YamlEmitter emitter, KeyValuePair<TKey, TValue> value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.BeginSequence();
        context.Serialize(ref emitter, value.Key);
        context.Serialize(ref emitter, value.Value);
        emitter.EndSequence();
    }

    public KeyValuePair<TKey, TValue> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
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
}

