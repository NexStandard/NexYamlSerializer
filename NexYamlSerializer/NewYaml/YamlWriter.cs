using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Serialization.PrimitiveSerializers;
using Stride.Core;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NexVYaml;

public class YamlWriter : IYamlWriter
{
    bool IsRedirected { get; set; } = false;
    bool IsFirst { get; set; } = true;
    IUtf8YamlEmitter Emitter { get; set; }
    IYamlFormatterResolver Resolver{ get; init; }
    internal YamlWriter(IUtf8YamlEmitter emitter, IYamlFormatterResolver resolver)
    {
        Resolver = resolver;
        Emitter = emitter;
    }

    public void BeginMapping(DataStyle style)
    {
        Emitter.BeginMapping(style);
    }

    public void BeginSequence(DataStyle style)
    {
        Emitter.BeginSequence(style);
    }

    public void EndMapping()
    {
        Emitter.EndMapping();
    }

    public void EndSequence()
    {
        Emitter.EndSequence();
    }

    public void Write(ReadOnlySpan<byte> value, DataStyle style = DataStyle.Any)
    {
        Emitter.WriteScalar(value);
    }
    public void Write<T>(T value, DataStyle style)
    {
        if (value is null)
        {
            Write(YamlCodes.Null0);
            return;
        }
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Serialize(this, value, style);
            return;
        }
        var type = typeof(T);

        if (type.IsValueType || type.IsSealed)
        {
            var formatter = Resolver.GetFormatter<T>();
            formatter.Serialize(this, value, style);
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = Resolver.GetFormatter(value!.GetType(), typeof(T));
            if (valueType != type)
                IsRedirected = true;

            // C# forgets the cast of T when invoking Serialize,
            // this way we can call the serialize method with the "real type"
            // that is in the object
            if (style is DataStyle.Any)
            {
                formatt.Serialize(this, value!);
            }
            else
            {
                formatt.Serialize(this, value!, style);
            }
        }
        else
        {
            if (style is DataStyle.Any)
            {
                Resolver.GetFormatter<T>().Serialize(this, value!);
            }
            else
            {
                Resolver.GetFormatter<T>().Serialize(this, value!, style);
            }
        }
    }
    public void Write(string? value, DataStyle style = DataStyle.Any)
    {
        if(value is null)
        {
            Write(YamlCodes.Null0);
            return;
        }
        var result = EmitStringAnalyzer.Analyze(value);
        var scalarStyle = result.SuggestScalarStyle();
        if (scalarStyle is ScalarStyle.Plain or ScalarStyle.Any)
        {
            Span<byte> span = stackalloc byte[value.Length];
            StringEncoding.Utf8.GetBytes(value, span);
            Write(span);
        }
        else if (ScalarStyle.Folded == scalarStyle)
        {
            throw new NotSupportedException($"The {ScalarStyle.Folded} is not supported.");
        }
        else if (ScalarStyle.SingleQuoted == scalarStyle)
        {
            throw new InvalidOperationException("Single Quote is reserved for char");
        }
        else if (ScalarStyle.DoubleQuoted == scalarStyle)
        {
            var scalarStringBuilt = EmitStringAnalyzer.BuildQuotedScalar(value, true);
            var stringConverted = scalarStringBuilt.ToString();
            Span<byte> span = stackalloc byte[stringConverted.Length];
            StringEncoding.Utf8.GetBytes(stringConverted, span);
            Write(span);
        }
        else if (ScalarStyle.Literal == scalarStyle)
        {
            var indentCharCount = (Emitter.CurrentIndentLevel + 1) * Utf8YamlEmitter.IndentWidth;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            Span<char> scalarChars = stackalloc char[scalarStringBuilt.Length];
            scalarStringBuilt.CopyTo(0, scalarChars, scalarStringBuilt.Length);

            scalarChars = Emitter.TryRemoveDuplicateLineBreak(scalarChars);

            var maxByteCount = StringEncoding.Utf8.GetMaxByteCount(scalarChars.Length);
            var offset = 0;
            var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(maxByteCount));
            Emitter.BeginScalar(output, ref offset);
            offset += StringEncoding.Utf8.GetBytes(scalarChars, output[offset..]);
            Emitter.EndScalar(output, ref offset);
        }
    }
    public void Write(char value, DataStyle style = DataStyle.Any)
    {
        var scalarStringBuilt = EmitStringAnalyzer.BuildQuotedScalar(value.ToString(), false);
        var stringConverted = scalarStringBuilt.ToString();
        Span<byte> span = stackalloc byte[stringConverted.Length];
        StringEncoding.Utf8.GetBytes(stringConverted, span);
        Write(span);
    }

    public void Write(short value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(int value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(uint value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(long value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }
    public void Write(ulong value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[20];

        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }
    public void Write(float value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(double value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(bool value, DataStyle style = DataStyle.Any)
    {
        Write(value ? YamlCodes.True0 : YamlCodes.False0);
    }

    public void Write(ushort value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(byte value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(sbyte value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }

    public void Write(decimal value, DataStyle style = DataStyle.Any)
    {
        Span<byte> span = stackalloc byte[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        Write(span[..written]);
    }
    public void WriteTag(string tag)
    {
        if (IsRedirected || IsFirst)
        {
            var fulTag = $"!{tag}";
            Emitter.Tag(ref fulTag);
            IsRedirected = false;
            IsFirst = false;
        }
    }


}
