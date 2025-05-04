using NexYaml.Core;
using Stride.Core;
using System.Globalization;

namespace NexYaml;

public static class YamlWriterExtensions
{
    public static MappingScopeDisposable MappingScope(this IYamlWriter stream, string tag, DataStyle style) => new(stream, tag, style);
    
    public static SequenceScopeDisposable SequenceScope(this IYamlWriter stream, string tag, DataStyle style) => new(stream, tag, style);

    public struct SequenceScopeDisposable : IDisposable
    {
        private IYamlWriter _stream;
        
        public SequenceScopeDisposable(IYamlWriter stream, string tag, DataStyle style)
        {
            _stream = stream;
            _stream.BeginSequence(tag, style);
        }

        public void Dispose() => _stream.End();
    }

    public struct MappingScopeDisposable : IDisposable
    {
        private IYamlWriter _stream;
        
        public MappingScopeDisposable(IYamlWriter stream, string tag, DataStyle style)
        {
            _stream = stream;
            _stream.BeginMapping(tag, style);
        }

        public void Dispose() => _stream.End();
    }

    public static void WriteEmptySequence(this IYamlWriter writer, string tag)
    {
        writer.WriteScalar($"{tag} [ ]");
    }
    public static void WriteEmptyMapping(this IYamlWriter writer, string tag)
    {
        writer.WriteScalar($"{tag} {{ }}");
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

    /// <summary>
    /// Writes a scalar value from a byte span to the YAML stream.
    /// </summary>
    /// <param name="stream">The YAML writer instance.</param>
    /// <param name="value">The byte span to write.</param>
    /// <param name="style">The data style to use. Default is <see cref="DataStyle.Any"/>.</param>
    public static void Write(this IYamlWriter stream, ReadOnlySpan<byte> value, DataStyle style = DataStyle.Any)
    {
        stream.WriteScalar(value);
    }

    /// <summary>
    /// Writes a value of type <typeparamref name="T"/> using the associated type handler.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="stream">The YAML writer instance.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="style">The data style to use.</param>
    public static void Write<T>(this IYamlWriter stream, T value, DataStyle style)
    {
        stream.WriteType(value, style);
    }

    /// <summary>
    /// Writes a scalar value from a character span to the YAML stream.
    /// </summary>
    /// <param name="stream">The YAML writer instance.</param>
    /// <param name="value">The character span to write.</param>
    /// <param name="style">The data style to use. Default is <see cref="DataStyle.Any"/>.</param>
    public static void Write(this IYamlWriter stream, ReadOnlySpan<char> value, DataStyle style = DataStyle.Any)
    {
        stream.WriteScalar(value);
    }

    /// <summary>
    /// Writes a string value to the YAML stream.
    /// </summary>
    /// <param name="stream">The YAML writer instance.</param>
    /// <param name="value">The string to write. Null values are handled accordingly.</param>
    /// <param name="style">The data style to use. Default is <see cref="DataStyle.Any"/>.</param>
    public static void Write(this IYamlWriter stream, string? value, DataStyle style = DataStyle.Any)
    {
        stream.WriteString(value, style);
    }

    /// <summary>
    /// Writes a single character as a single quoted YAML scalar.
    /// </summary>
    /// <param name="stream">The YAML writer instance.</param>
    /// <param name="value">The character to write.</param>
    /// <param name="style">The data style to use. Default is <see cref="DataStyle.Any"/>.</param>
    public static void Write(this IYamlWriter stream, char value, DataStyle style = DataStyle.Any)
    {
        stream.Write(['\'', value, '\''], style);
    }

    public static void Write(this IYamlWriter stream, short value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, int value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, uint value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, long value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }
    public static void Write(this IYamlWriter stream, ulong value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[20];

        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }
    public static void Write(this IYamlWriter stream, float value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        stream.Write(span[..written]);
    }

    public static void Write(this IYamlWriter stream, double value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[32];
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
        stream.Write(value ? ['t', 'r', 'u', 'e'] : stackalloc[] { 'f', 'a', 'l', 's', 'e' });
    }

    public static void Write(this IYamlWriter stream, ushort value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[5];
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
        Span<char> span = stackalloc char[3];
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
        Span<char> span = stackalloc char[4];
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
        Span<char> span = stackalloc char[64];
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
    public static void Write(this IYamlWriter stream, string key, sbyte value, DataStyle style = DataStyle.Any)
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
    public static void Write(this IYamlWriter stream, string key, decimal value, DataStyle style = DataStyle.Any)
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

