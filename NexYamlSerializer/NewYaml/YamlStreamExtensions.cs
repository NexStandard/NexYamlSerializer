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

    public static void Write(this IYamlWriter stream, string key, int value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, uint value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, long value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, float value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, double value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, bool value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, short value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, ushort value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, byte value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, sbyte value, DataStyle style = DataStyle.Any)
    {
        writer.Write(key);
        writer.Write(value, style);
    }

    public static void Write(this IYamlWriter writer, string key, decimal value, DataStyle style = DataStyle.Any)
    {
        writer.Write(key);
        writer.Write(value, style);
    }

    public static void Write<T>(this IYamlWriter stream, string key, T value, DataStyle style)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, string key, ulong value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value);
    }

    public static void Write(this IYamlWriter stream, string key, char value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value);
    }

    public static void Write(this IYamlWriter stream,string key, string? value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        if (value is null)
        {
            stream.Write(YamlCodes.Null0);
        }
        else
        {
            stream.Write(value);
        }
    }
}

