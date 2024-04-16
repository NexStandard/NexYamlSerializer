using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
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

    public static void Serialize<T>(this ISerializationWriter stream, T value, DataStyle style = DataStyle.Any)
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
                var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;
                arrayFormatter.Serialize(stream, value, style);
            }
            else
            {
                stream.SerializeContext.Serialize(stream, value, style);
            }
        }
    }

    public static void Serialize<T, K>(this ISerializationWriter stream, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.BeginSequence(style);
        stream.Serialize(value.Key);
        stream.Serialize(value.Value);
        stream.EndSequence();
    }

    public static void Serialize<T>(this ISerializationWriter stream, string key, T value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(key);
        stream.Serialize(value, style);
    }
}

