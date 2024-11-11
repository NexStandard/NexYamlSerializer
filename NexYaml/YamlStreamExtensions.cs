using NexYaml.Core;
using Stride.Core;
using System.Globalization;

namespace NexYaml;

public static class YamlStreamExtensions
{
    /// <summary>
    /// Run the <paramref name="action"/> on the stream while wrapping it into <see cref="IYamlWriter.BeginMapping(DataStyle)"
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="style">The <see cref="DataStyle"/> to format the mapping</param>
    /// <param name="action">The Action to run inside the mapping</param>
    public static void WriteMapping(this IYamlWriter stream, DataStyle style, Action action)
    {
        stream.BeginMapping(style);
        action();
        stream.EndMapping();
    }

    /// <summary>
    /// Run the <paramref name="action"/> on the stream while wrapping it into <see cref="IYamlWriter.BeginSequence(DataStyle)"
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="style">The <see cref="DataStyle"/> to format the mapping</param>
    /// <param name="action">The Action to run inside the mapping</param>
    public static void WriteSequence(this IYamlWriter stream, DataStyle style, Action action)
    {
        stream.BeginSequence(style);
        action();
        stream.EndSequence();
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
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
        stream.Write(['\'', value, '\''], style);
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

    /// <summary>
    /// Writes the <paramref name="value"/> of type <see cref="decimal"/> to the stream as scalar
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="value">The <see cref="decimal"/> to write to the stream</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
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

    /// <summary>
    /// Writes the <paramref name="value"/> of type <see cref="byte"/> to the stream as scalar
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="value">The <see cref="byte"/> to write to the stream</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, byte value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    /// <summary>
    /// Writes the <paramref name="value"/> of type <see cref="sbyte"/> to the stream as scalar
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="value">The <see cref="sbyte"/> to write to the stream</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, sbyte value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    /// <summary>
    /// Writes the <paramref name="value"/> of type <see cref="decimal"/> to the stream as scalar
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="value">The <see cref="decimal"/> to write to the stream</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, decimal value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, uint value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, long value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, float value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, double value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, bool value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, short value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, ushort value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, byte value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter writer, string key, sbyte value, DataStyle style = DataStyle.Any)
    {
        writer.Write(key);
        writer.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter writer, string key, decimal value, DataStyle style = DataStyle.Any)
    {
        writer.Write(key);
        writer.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write<T>(this IYamlWriter stream, string key, T value, DataStyle style)
    {
        stream.Write(key);
        stream.Write(value, style);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, ulong value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, char value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key);
        stream.Write(value);
    }

    /// <summary>
    /// Writes the YAML scalar of <paramref name="key"/> and the <paramref name="value"/>
    /// </summary>
    /// <param name="stream">The <see cref="IYamlWriter"/> to write to</param>
    /// <param name="key">The key of the scalar</param>
    /// <param name="value">The value of the scalar</param>
    /// <param name="style">The <see cref="DataStyle"/> of the scalar</param>
    public static void Write(this IYamlWriter stream, string key, string? value, DataStyle style = DataStyle.Any)
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

