using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using Stride.Core;

namespace NexYaml.Serialization;
internal readonly record struct EmitResult(IEmitter Emitter);
public readonly record struct WriteContext(IEmitter Emitter, int Indent, bool IsRedirected, IYamlWriter Stream)
{

}

public static class Writing
{
    public static WriteContext WriteScalar(in this WriteContext context, in ReadOnlySpan<char> value)
    {
        return context with
        {
            Emitter = context.Emitter.WriteScalar(value)
        };
    }
    public static WriteContext BeginMapping(in this WriteContext write, string tag, DataStyle style)
    {
        IEmitter emitter = null!;
        write.Emitter.StateMachine.StyleEnforcer.Begin(ref style);
        if (style is DataStyle.Normal or DataStyle.Any)
        {
            emitter = write.Emitter.StateMachine.blockMapKeySerializer;
        }
        else if (style is DataStyle.Compact)
        {
            emitter = write.Emitter.StateMachine.flowMapKeySerializer;
        }
        var context = new BeginContext()
        {
            Emitter = write.Emitter,
            Indentation = emitter.StateMachine.IndentationManager,
            NeedsTag = false
        };
        if (write.IsRedirected)
        {
            context = new BeginContext()
            {
                NeedsTag = write.IsRedirected,
                Tag = tag,
                Emitter = write.Emitter,
                Indentation = emitter.StateMachine.IndentationManager
            };
        }
        emitter = emitter.Begin(context);
        return write with
        {
            IsRedirected = false,
            Emitter = emitter,
            Indent = write.Indent + 2
        };
    }
    public static WriteContext BeginSequence(in this WriteContext write, string tag, DataStyle style)
    {
        IEmitter emitter = null!;
        write.Emitter.StateMachine.StyleEnforcer.Begin(ref style);
        if (style is DataStyle.Normal or DataStyle.Any)
        {
            emitter = write.Emitter.StateMachine.blockSequenceEntrySerializer;
        }
        else if (style is DataStyle.Compact)
        {
            emitter = write.Emitter.StateMachine.flowSequenceEntrySerializer;
        }
        var context = new BeginContext()
        {
            Emitter = write.Emitter,
            Indentation = emitter.StateMachine.IndentationManager,
            NeedsTag = false
        };
        if (write.IsRedirected)
        {
            context = new BeginContext()
            {
                NeedsTag = write.IsRedirected,
                Tag = tag,
                Emitter = write.Emitter,
                Indentation = emitter.StateMachine.IndentationManager
            };
        }
        emitter = emitter.Begin(context);
        return write with
        {
            IsRedirected = false,
            Emitter = emitter,
            Indent = write.Indent + 2
        };
    }
    public static WriteContext End(in this WriteContext context,in WriteContext current)
    {
        var result = context.Emitter.End(current.Emitter);
        current.Emitter.StateMachine.StyleEnforcer.End();
        return context with { Emitter = result };
    }
    public static WriteContext WriteEmptySequence(in this WriteContext context, string tag)
    {
        return context with { Emitter = context.Emitter.WriteScalar($"{tag} [ ]") };
    }
    public static WriteContext WriteEmptyMapping(in this WriteContext context, string tag)
    {
        return context with { Emitter = context.Emitter.WriteScalar($"{tag} {{ }}") };
    }
    public static WriteContext Write<T>(this WriteContext context, in T value, DataStyle style)
    {
        foreach (var syntax in context.Stream.Plugins)
        {
            if (syntax.Write(context.Stream, value, style, context, out var newContext))
            {
                return newContext;
            }
        }
        var type = typeof(T);

        if (type.IsValueType || type.IsSealed)
        {
            return context.Stream.Resolver.GetSerializer<T>().Write(context.Stream, value, style, context);
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = context.Stream.Resolver.GetSerializer(value!.GetType(), typeof(T));
            if (valueType != type)
            {
                context = context with
                {
                    IsRedirected = true
                };
            }

            // C# forgets the cast of T when invoking Serialize,
            // this way we can call the serialize method with the "real type"
            // that is in the object
            if (style is DataStyle.Any)
            {
                return formatt.Write(context.Stream, value!, context);
            }
            else
            {
                return formatt.Write(context.Stream, value!, style, context);
            }
        }
        else
        {
            if (style is DataStyle.Any)
            {
                return context.Stream.Resolver.GetSerializer<T>().Write(context.Stream, value!, context);
            }
            else
            {
                return context.Stream.Resolver.GetSerializer<T>().Write(context.Stream, value!, style, context);
            }
        }
    }
    /// <summary>
    /// Writes a string value to the YAML stream.
    /// </summary>
    /// <param name="stream">The YAML writer instance.</param>
    /// <param name="value">The string to write. Null values are handled accordingly.</param>
    /// <param name="style">The data style to use. Default is <see cref="DataStyle.Any"/>.</param>
    public static WriteContext Write(in this WriteContext stream, string? value, DataStyle style = DataStyle.Any)
    {
        if (value is null)
        {
            return stream.WriteScalar(YamlCodes.Null);
        }
        var result = EmitStringAnalyzer.Analyze(value);
        var scalarStyle = result.SuggestScalarStyle();
        if (scalarStyle is ScalarStyle.Plain or ScalarStyle.Any)
        {
            return stream.WriteScalar(value);
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
            return stream.WriteScalar("\"" + value + "\"");
        }
        else if (ScalarStyle.Literal == scalarStyle)
        {
            var indentCharCount = (stream.Indent + 1) * stream.Indent;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            return stream.WriteScalar(scalarStringBuilt.ToString());
        }
        // TODO is this reachable?
        throw new YamlException("Couldnt get ScalarStyle");
    }
    public static WriteContext Write(in this WriteContext context, in ReadOnlySpan<char> value, DataStyle style = DataStyle.Any)
    {
        return context.WriteScalar(value);
    }

    public static WriteContext Write(in this WriteContext context, in ReadOnlySpan<byte> value, DataStyle style = DataStyle.Any)
    {
        ReadOnlySpan<char> byteSpan = Encoding.UTF8.GetChars(value.ToArray());
        return context.WriteScalar(byteSpan);
    }
    public static WriteContext Write(in this WriteContext context, char value, DataStyle style = DataStyle.Any)
    {
        return context.WriteScalar(['\'', value, '\'']);
    }

    public static WriteContext Write(in this WriteContext context, short value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, int value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, uint value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, long value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }
    public static WriteContext Write(in this WriteContext context, ulong value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[20];

        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }
    public static WriteContext Write(in this WriteContext context, float value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, double value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, bool value, DataStyle style = DataStyle.Any)
    {
        return context.WriteScalar(value ? ['t', 'r', 'u', 'e'] : stackalloc[] { 'f', 'a', 'l', 's', 'e' });
    }

    public static WriteContext Write(in this WriteContext context, ushort value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, byte value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, sbyte value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, decimal value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return context.WriteScalar(span[..written]);
    }

    public static WriteContext Write(in this WriteContext context, string key, uint value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, long value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, float value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, double value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, bool value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, short value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, ushort value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, byte value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, sbyte value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, decimal value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write<T>(in this WriteContext context, string key, T value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value, style);
    }

    public static WriteContext Write(in this WriteContext context, string key, ulong value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value);
    }

    public static WriteContext Write(in this WriteContext context, string key, char value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value);
    }

    public static WriteContext Write(in this WriteContext context, string key, string? value, DataStyle style = DataStyle.Any)
    {
        return context.Write(key).Write(value);
    }
}