using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Serialization;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;
using System;
using System.Runtime.CompilerServices;
using ScalarStyle = NexVYaml.Emitter.ScalarStyle;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void Write<T>(this IYamlStream stream, string key, ref T value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        if(value == null)
        {
            stream.WriteNull();
        }
        else
        {
            if(value is Array)
            {
                new ArrayFormatter<T>().Serialize(ref stream,value,style);
            }
            else
            {
                stream.SerializeContext.Serialize(ref stream, value, style);
            }
        }
    }
    public static void Write<T>(this IYamlStream stream, string key, ref T? value, DataStyle style = DataStyle.Any)
        where T : struct
    {
        stream.Serialize(ref key);
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            stream.SerializeContext.Serialize(ref stream, value, style);
        }
    }
    public static void Write<T>(this IYamlStream stream, string key, ref T[] value, DataStyle style = DataStyle.Any)
    {
        if (style is DataStyle.Any)
        {
            style = DataStyle.Normal;
        }
        stream.Serialize(ref key);
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            stream.Emitter.BeginSequence(style);
            foreach (var item in value)
            {
                var x = item;
                stream.Write(ref x,style);
            }
            stream.Emitter.EndSequence();
        }
    }
    public static void Write<T>(this IYamlStream stream, ref T[] value, DataStyle style = DataStyle.Any)
    {
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            if(typeof(T).IsValueType)
            {

            }
            var contentStyle = DataStyle.Any;
            if (style == DataStyle.Compact)
            {
                contentStyle = DataStyle.Compact;
            }
            stream.Emitter.BeginSequence(style);
            foreach (var x in value)
            {
                var val = x;
                stream.Write(ref val, contentStyle);
            }
            stream.Emitter.EndSequence();
        }
    }
    public static void Write<T>(this IYamlStream stream, T[] value, DataStyle style = DataStyle.Any)
    {
        stream.Write(ref value, style);
    }
    public static void Write<T>(this IYamlStream stream, T?[] value, DataStyle style = DataStyle.Any)
        where T : struct
    {
        stream.Write(ref value, style);
    }

    public static void Write<T>(this IYamlStream stream, string key, T[] value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key, ref value, style);
    }

    public static void Write<T>(this IYamlStream stream, string key, T value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        if(style == DataStyle.Any)
        {
            style = DataStyle.Normal;
        }
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            stream.Write(ref value,style);
        }
    }
    public static void Write<T>(this IYamlStream stream, string key, T? value, DataStyle style = DataStyle.Any)
        where T : struct
    {
        stream.Serialize(ref key);
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.SerializeContext.Serialize(ref stream, val, style);
        }
    }
    public static void WriteTag(this IYamlStream stream, string tag)
    {
        if (stream.SerializeContext.IsRedirected || stream.SerializeContext.IsFirst)
        {
            stream.Emitter.Tag($"!{tag}");
            stream.SerializeContext.IsRedirected = false;
            stream.SerializeContext.IsFirst = false;
        }
    }
    public static void Write<T>(this IYamlStream stream, ref T? value, DataStyle style = DataStyle.Any)
    {
        if (style is DataStyle.Any)
        {
            style = DataStyle.Normal;
        }
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            if (value is Array)
            {
                var t = typeof(T).GetElementType();
                var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
                YamlSerializer arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType);
                arrayFormatter.Serialize(ref stream, value, style);
            }
            else
            {
                stream.SerializeContext.Serialize(ref stream, value, style);
            }
        }
    }
    public static void Write<T>(this IYamlStream stream, T? value, DataStyle style = DataStyle.Any)
    {
        if (value == null)
            stream.WriteNull();
        else
            stream.Write(ref value,style);
    }

    public static void Write(this IYamlStream stream, ref string? value)
    {
        if(value == null)
            stream.WriteNull();
        else
            stream.Serialize(ref value);
    }

    public static void WriteNull(this IYamlStream stream)
    {
        stream.Serialize(YamlCodes.Null0);
    }

    public static void Write(this IYamlStream stream, string? value, ScalarStyle style = ScalarStyle.Any)
    {
        if(value == null)
            stream.WriteNull();
        else
            stream.Serialize(ref value);
    }
    public static void Write(this IYamlStream stream, ref string value, ScalarStyle style = ScalarStyle.Any)
    {
        if (value == null)
            stream.WriteNull();
        else
            stream.Serialize(ref value);
    }
    public static void Write(this IYamlStream stream, string key, string? value, ScalarStyle style = ScalarStyle.Any)
    {
        stream.Write(key, ref value, style);
    }
    public static void Write(this IYamlStream stream, string Key, ref string? value, ScalarStyle style = ScalarStyle.Any)
    {
        stream.Serialize(ref Key);
        if(value == null)
        {
            stream.WriteNull();
            return;
        }
        if (style == ScalarStyle.Any)
        {
            var analyzeInfo = EmitStringAnalyzer.Analyze(value);
            style = analyzeInfo.SuggestScalarStyle();
        }
        var writer = new StringWriter(stream.Emitter);
        switch (style)
        {
            case ScalarStyle.Plain:
                stream.Serialize(ref value);
                break;
            case ScalarStyle.SingleQuoted:
                writer.WriteQuotedScalar(value, doubleQuote: false);
                break;
            case ScalarStyle.DoubleQuoted:
                writer.WriteQuotedScalar(value, doubleQuote: true);
                break;
            case ScalarStyle.Literal:
                writer.WriteLiteralScalar(value);
                break;
            case ScalarStyle.Folded:
                throw new NotSupportedException();

            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }

}

