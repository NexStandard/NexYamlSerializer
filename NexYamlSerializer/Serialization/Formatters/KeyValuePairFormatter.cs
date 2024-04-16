#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class KeyValuePairFormatter<TKey, TValue> : YamlSerializer<KeyValuePair<TKey, TValue>>
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
        stream.BeginSequence(style);
        stream.Serialize(value.Key);
        stream.Serialize(value.Value);
        stream.EndSequence();
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

    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(KeyValuePairFormatter<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

