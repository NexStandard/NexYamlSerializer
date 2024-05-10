using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteNull(this SerializationWriter stream)
    {
        ReadOnlySpan<byte> nullTag = YamlCodes.Null0;
        stream.Serialize(ref nullTag);
    }

    public static void Write<T>(this SerializationWriter stream, T value, DataStyle style = DataStyle.Any)
    {
        if (value is null)
        {
            ReadOnlySpan<byte> buf = YamlCodes.Null0;
            stream.Serialize(ref buf);
            return;
        }
        if (value is Array)
        {
            var t = typeof(T).GetElementType();
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t!);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Serialize(stream, value, style);
            return;
        }
        if (style is DataStyle.Any)
        {
            stream.SerializeContext.Serialize(stream, value);
        }
        else
        {
            stream.SerializeContext.Serialize(stream, value, style);
        }
    }

    public static void Write<T, K>(this SerializationWriter stream, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.BeginSequence(style);
        stream.Write(value.Key);
        stream.Write(value.Value);
        stream.EndSequence();
    }    
    public static void Write<T, K>(this SerializationWriter stream, string key, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
    public static void Write<T, K>(this SerializationWriter stream, Dictionary<T, K> value, DataStyle style = DataStyle.Any)
        where T : notnull
    {
        if (value is null)
            stream.WriteNull();
        else
            DictionaryFormatterHelper.Serialize(stream, value, style);
    }
    public static void Write<T, K>(this SerializationWriter stream, string key, Dictionary<T, K> value, DataStyle style)
        where T : notnull
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
    public static void Write<T>(this SerializationWriter stream, List<T> value, DataStyle style)
    {
        if (value is null)
            stream.WriteNull();
        else
            ListFormatterHelper.Serialize(stream, value, style);
    }
    public static void Write<T>(this SerializationWriter stream, string key, List<T> value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
    public static void Write<T>(this SerializationWriter stream, string key, T value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }

    public static void Write(this SerializationWriter stream,string key, sbyte value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream,string key, int value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, uint value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, long value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, ulong value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, float value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, double value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, short value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, ushort value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, char value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, bool value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream,string key, string value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            ReadOnlySpan<byte> nullTag = YamlCodes.Null0;
            stream.Serialize(ref nullTag);
        }
        else
        {
            stream.Serialize(ref value);
        }
    }

    public static void Write(this SerializationWriter stream,string key, decimal value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this SerializationWriter stream, string key, byte value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }
}

