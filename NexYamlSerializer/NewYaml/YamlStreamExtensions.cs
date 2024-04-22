using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteNull(this ISerializationWriter stream)
    {
        stream.Serialize(YamlCodes.Null0);
    }

    public static void Write<T>(this ISerializationWriter stream, T value, DataStyle style = DataStyle.Any)
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
                if(style is DataStyle.Any)
                {
                    stream.SerializeContext.Serialize(stream, value);
                }
                else
                {
                    stream.SerializeContext.Serialize(stream, value, style);
                }
            }
        }
    }

    public static void Write<T, K>(this ISerializationWriter stream, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.BeginSequence(style);
        stream.Write(value.Key);
        stream.Write(value.Value);
        stream.EndSequence();
    }    
    public static void Write<T, K>(this ISerializationWriter stream, string key, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        
        stream.Write(value);
    }
    public static void Serialize<T, K>(this ISerializationWriter stream, Dictionary<T, K> value, DataStyle style = DataStyle.Any)
    {
        if (value is null)
            stream.WriteNull();
        else
            DictionaryFormatterHelper.Serialize(stream, value, style);
    }
    public static void Write<T, K>(this ISerializationWriter stream, string key, Dictionary<T, K> value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(value, style);
    }
    public static void Write<T>(this ISerializationWriter stream, List<T> value, DataStyle style)
    {
        if (value is null)
            stream.WriteNull();
        else
            ListFormatterHelper.Serialize(stream, value, style);
    }
    public static void Write<T>(this ISerializationWriter stream, string key, List<T> value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
    public static void Write<T>(this ISerializationWriter stream, string key, T value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }

    public static void Write(this ISerializationWriter stream,string key, sbyte value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream,string key, int value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, uint value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, long value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ulong value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, float value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, double value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, short value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ushort value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, char value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, bool value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream,string key, string value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.Serialize(YamlCodes.Null0);
        }
        else
        {
            stream.Serialize(ref value);
        }
    }

    public static void Write(this ISerializationWriter stream,string key, decimal value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, byte value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }
}

