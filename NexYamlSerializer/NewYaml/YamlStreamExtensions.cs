using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Serialization.PrimitiveSerializers;
using Stride.Core;
using Stride.Core.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteNull(this IYamlWriter stream)
    {
        ReadOnlySpan<byte> nullTag = YamlCodes.Null0;
        stream.Serialize(nullTag);
    }
    public static void Write<T>(this IYamlWriter stream, T value, DataStyle style)
    {
        stream.Serialize(ref value, style);
    }

    public static void Write(this IYamlWriter writer, int value, DataStyle style = DataStyle.Any)
    {
        Int32Formatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, uint value, DataStyle style = DataStyle.Any)
    {
        UInt32Formatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, long value, DataStyle style = DataStyle.Any)
    {
        Int64Formatter.Instance.Serialize(writer, value, style);
    }
    public static void Write(this IYamlWriter writer, ulong value, DataStyle style = DataStyle.Any)
    {
        UInt64Formatter.Instance.Serialize(writer, value, style);
    }
    public static void Write(this IYamlWriter writer, float value, DataStyle style = DataStyle.Any)
    {
        Float32Formatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, double value, DataStyle style = DataStyle.Any)
    {
        Float64Formatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, bool value, DataStyle style = DataStyle.Any)
    {
        BooleanFormatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, short value, DataStyle style = DataStyle.Any)
    {
        Int16Formatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, ushort value, DataStyle style = DataStyle.Any)
    {
        UInt16Formatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, byte value, DataStyle style = DataStyle.Any)
    {
        ByteFormatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, sbyte value, DataStyle style = DataStyle.Any)
    {
        SByteFormatter.Instance.Serialize(writer,value,style);
    }

    public static void Write(this IYamlWriter writer, decimal value, DataStyle style = DataStyle.Any)
    {
        DecimalFormatter.Instance.Serialize(writer, value, style);
    }

    public static void Write(this IYamlWriter writer, string key, int value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, uint value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, long value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, float value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, double value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, bool value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, short value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, ushort value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, byte value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, sbyte value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, decimal value, DataStyle style = DataStyle.Any)
    {
        writer.Serialize(ref key);
        writer.Write(value, style);
    }
    public static void Write<T, K>(this IYamlWriter stream, string key, Dictionary<T, K> value, DataStyle style)
        where T : notnull
    {
        stream.Serialize(ref key);
        var ser = new DictionaryFormatter<T, K>();
        ser.Serialize(stream, value, style);
    }
    public static void Write<T>(this IYamlWriter stream, string key, List<T> value, DataStyle style)
    {
        stream.Serialize(ref key);
        var serializer = new ListFormatter<T>();
        serializer.Serialize(stream, value, style);
    }
    public static void Write<T>(this IYamlWriter stream, string key, T value, DataStyle style)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value, style);
    }

    public static void Write(this IYamlWriter stream, string key, ulong value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(value);
    }

    public static void Write(this IYamlWriter stream, string key, char value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlWriter stream,string key, string? value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            ReadOnlySpan<byte> nullTag = YamlCodes.Null0;
            stream.Serialize(nullTag);
        }
        else
        {
            stream.Serialize(ref value);
        }
    }
}

