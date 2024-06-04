using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using Stride.Core.Serialization.Serializers;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteNull(this ISerializationWriter stream)
    {
        ReadOnlySpan<byte> nullTag = YamlCodes.Null0;
        stream.Serialize(ref nullTag);
    }
    public static void Write<T>(this ISerializationWriter stream, T value, DataStyle style)
    {
        stream.Serialize(ref value, style);
    }
    public static void Write<T, K>(this ISerializationWriter stream, string key, Dictionary<T, K> value, DataStyle style)
        where T : notnull
    {
        stream.Serialize(ref key);
        var ser = new DictionaryFormatter<T, K>();
        ser.Serialize(stream, value, style);
    }
    public static void Write<T>(this ISerializationWriter stream, string key, List<T> value, DataStyle style)
    {
        stream.Serialize(ref key);
        var serializer = new ListFormatter<T>();
        serializer.Serialize(stream, value, style);
    }
    public static void Write<T>(this ISerializationWriter stream, string key, T value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value, style);
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

    public static void Write(this ISerializationWriter stream,string key, string? value, DataStyle style = DataStyle.Any)
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

