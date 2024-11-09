using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Serialization.PrimitiveSerializers;
using Stride.Core;
using Stride.Core.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NexVYaml;

public static class YamlStreamExtensions
{

    public static void Write(this IYamlWriter stream, string key, int value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    public static void Write(this IYamlWriter stream, ReadOnlySpan<byte> value, DataStyle style = DataStyle.Any)
    {
        stream.WriteScalar(value);
    }

    public static void Write<T>(this IYamlWriter stream, T value, DataStyle style)
    {
        stream.WriteType(value, style);
    }
    public static void Write(this IYamlWriter stream, ReadOnlySpan<char> value, DataStyle style = DataStyle.Any)
    {
        stream.WriteScalar(value);
    }
    public static void Write(this IYamlWriter stream, string? value, DataStyle style = DataStyle.Any)
    {
        stream.WriteString(value, style);
    }
    public static void Write(this IYamlWriter stream, char value, DataStyle style = DataStyle.Any)
    {
        stream.Write(['\'',value,'\''],style);
    }

    public static void Write(this IYamlWriter stream, short value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, int value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, uint value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, long value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }
    public static void Write(this IYamlWriter stream, ulong value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[20];

        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }
    public static void Write(this IYamlWriter stream, float value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, double value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, bool value, DataStyle style = DataStyle.Any)
    {
        stream.Write(value ? [(byte)'t', (byte)'r', (byte)'u', (byte)'e'] : stackalloc[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' });
    }

    public static void Write(this IYamlWriter stream, ushort value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, byte value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, sbyte value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, decimal value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
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

