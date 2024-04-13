using NexVYaml.Serialization;
using NexYamlSerializer;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensionsDictionary
{
    public static void Write<TKey,TValue>(this ISerializationWriter stream, Dictionary<TKey,TValue> value, DataStyle style = DataStyle.Any)
    {
        if(value is null)
        {
            stream.WriteNull();
            return;
        }

        YamlSerializer<TKey> keyFormatter = null;
        YamlSerializer<TValue> valueFormatter = null;
        if (FormatterExtensions.IsPrimtiveType(typeof(TKey)))
        {
            keyFormatter = NewSerializerRegistry.Instance.GetFormatter<TKey>();
        }
        if (FormatterExtensions.IsPrimtiveType(typeof(TValue)))
            valueFormatter = NewSerializerRegistry.Instance.GetFormatter<TValue>();

        if (keyFormatter == null)
        {
            stream.Emitter.BeginSequence();
            if (value.Count > 0)
            {
                var elementFormatter = new KeyValuePairFormatter<TKey, TValue>();
                foreach (var x in value)
                {
                    elementFormatter.Serialize(ref stream, x);
                }
            }
            stream.Emitter.EndSequence();
        }
        else if (valueFormatter == null)
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    stream.Write(x.Value);
                }
            }
            stream.Emitter.EndMapping();
        }
        else
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    valueFormatter.Serialize(ref stream, x.Value, style);
                }
            }
            stream.Emitter.EndMapping();
        }
    }

    public static void Write<T,K>(this ISerializationWriter stream, string key, Dictionary<T,K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
}