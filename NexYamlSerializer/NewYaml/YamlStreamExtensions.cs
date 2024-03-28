using NexVYaml.Emitter;
using NexVYaml.Internal;
using Stride.Core;
using System;
using System.Runtime.CompilerServices;
using ScalarStyle = NexVYaml.Emitter.ScalarStyle;

namespace NexYaml2.NewYaml;

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
            var emitter = stream.Emitter;
            stream.SerializeContext.Serialize(ref emitter, value, style);
        }
    }
    public static void Write<T>(this IYamlStream stream, ref T? value, DataStyle style = DataStyle.Any)
    {
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            var emitter = stream.Emitter;
            stream.SerializeContext.Serialize(ref emitter, value, style);
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

