using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteTag(this ISerializationWriter stream, string tag)
    {
        stream.SerializeTag(ref tag);
    }
    public static void Write<T>(this ISerializationWriter stream, ref T? value, DataStyle style = DataStyle.Any)
    {
        if (style is DataStyle.Any)
        {
            style = DataStyle.Normal;
        }
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            if (value is Array)
            {
                var t = typeof(T).GetElementType();
                var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
                YamlSerializer arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType);
                arrayFormatter.Serialize(ref stream, value, style);
            }
            else
            {
                stream.SerializeContext.Serialize(stream, value, style);
            }
        }
    }
    public static void Write<T>(this ISerializationWriter stream, T? value, DataStyle style = DataStyle.Any)
    {
        if (value == null)
            stream.WriteNull();
        else
            stream.Write(ref value,style);
    }

    public static void WriteNull(this ISerializationWriter stream)
    {
        stream.Serialize(YamlCodes.Null0);
    }
}

