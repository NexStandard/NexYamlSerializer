using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer;
using Stride.Core;
using Stride.Core.Serialization;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteTag(this ISerializationWriter stream, string tag)
    {
        stream.SerializeTag(ref tag);
    }
    public static void WriteNull(this ISerializationWriter stream)
    {
        stream.Serialize(YamlCodes.Null0);
    }

    public static void Serialize<T>(this ISerializationWriter stream,T value, DataStyle style = DataStyle.Any)
    {
        if (value == null)
        {
            ReadOnlySpan<byte> buf = YamlCodes.Null0;
            stream.Serialize(buf);
        }
        else
        {
            if (value is Array)
            {
                var t = typeof(T).GetElementType();
                var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
                var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType);
                arrayFormatter.Serialize(stream, value, style);
            }
            else
            {
                stream.SerializeContext.Serialize(stream, value, style);
            }
        }
    }

    public static void Serialize<T,K>(this ISerializationWriter stream, KeyValuePair<T,K> value, DataStyle style = DataStyle.Any)
    {
        stream.BeginSequence(style);
        stream.Serialize(value.Key);
        stream.Serialize(value.Value);
        stream.EndSequence();
    }    
    public static void Serialize<T,K>(this ISerializationWriter stream, string key,KeyValuePair<T,K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(value, style);
    }
    public static void Serialize<TKey, TValue>(this ISerializationWriter stream, string key, IDictionary<TKey, TValue> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(value, style);
    }
    
    public static void Serialize<TKey, TValue>(this ISerializationWriter stream, IDictionary<TKey, TValue> value, DataStyle style = DataStyle.Any)
    {
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
                foreach (var x in value)
                {
                    stream.Serialize(x, style);
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
                    stream.Serialize(x.Value);
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
    public static void Serialize<TKey, TValue>(this ISerializationWriter stream, string key, Dictionary<TKey, TValue> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize((IDictionary<TKey,TValue>)value, style);
    }
    public static void Serialize<TKey, TValue>(this ISerializationWriter stream, Dictionary<TKey, TValue> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize((IDictionary<TKey,TValue>)value, style);
    }
    public static void Serialize<T>(this ISerializationWriter stream, string key,List<T> value, DataStyle style= DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(value, style);
    }
    public static void Serialize<T>(this ISerializationWriter stream, List<T> value, DataStyle style = DataStyle.Any)
    {
        stream.BeginSequence(style);
        foreach (var x in value)
        {
            stream.Serialize(x, style);
        }
        stream.EndSequence();
    }
    public static void Serialize<T>(this ISerializationWriter stream, string key, T value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(key);
        stream.Serialize(value, style);
    }
}

