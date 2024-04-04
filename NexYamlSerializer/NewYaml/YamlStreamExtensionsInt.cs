using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensionsInt
{
    public static void Write(this IYamlStream stream, int value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, int? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, ref int value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ref int? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, string key, int? value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, int value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref int value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref int? value)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }
}
public static class YamlStreamExtensionsDouble
{
    public static void Write(this IYamlStream stream, double value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, double? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, ref double value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ref double? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, string key, double? value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, double value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref double value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref double? value)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }
}
public static class YamlStreamExtensionsulong
{
    public static void Write(this IYamlStream stream, ulong value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ulong? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, ref ulong value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ref ulong? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, string key, ulong? value)
    {
        stream.Serialize(ref key);
        if (value is null)
            stream.WriteNull();
        else
        {
            var x = value.Value;
            stream.Serialize(ref x);
        }
    }

    public static void Write(this IYamlStream stream, string key, ulong value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref ulong value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref ulong? value)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }
}
public static class YamlStreamExtensionsFloat
{
    public static void Write(this IYamlStream stream, float value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, float? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, ref float value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ref float? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, string key, float? value)
    {
        stream.Serialize(ref key);
        if (value is null)
            stream.WriteNull();
        else
        {
            var x = value.Value;
            stream.Serialize(ref x);
        }
    }

    public static void Write(this IYamlStream stream, string key, float value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref float value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref float? value)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }
}
public static class YamlStreamExtensionsbool
{
    public static void Write(this IYamlStream stream, bool value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, bool? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, ref bool value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ref bool? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, string key, bool? value)
    {
        stream.Serialize(ref key);
        if (value is null)
            stream.WriteNull();
        else
        {
            var x = value.Value;
            stream.Serialize(ref x);
        }
    }

    public static void Write(this IYamlStream stream, string key, bool value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref bool value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref bool? value)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }
}

public static class YamlStreamExtensionsuint
{
    public static void Write(this IYamlStream stream, uint value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, uint? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, ref uint value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, ref uint? value)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }

    public static void Write(this IYamlStream stream, string key, uint? value)
    {
        stream.Serialize(ref key);
        if(value is null)
            stream.WriteNull();
        else
        {
            var x = value.Value;
            stream.Serialize(ref x);
        }
    }

    public static void Write(this IYamlStream stream, string key, uint value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref uint value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this IYamlStream stream, string key, ref uint? value)
    {
        stream.Serialize(ref key);
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            var val = value.Value;
            stream.Serialize(ref val);
        }
    }
}
public static class YamlStreamExtensionsUri
{
    public static void Write(this IYamlStream stream, Uri value)
    {
        var s = value.ToString();
        stream.Serialize(ref s);
    }

    public static void Write(this IYamlStream stream, ref Uri value)
    {
        var s = value.ToString();
        stream.Serialize(ref s);
    }

    public static void Write(this IYamlStream stream, string key, Uri value)
    {
        var s = value.ToString();
        stream.Write(key, ref s);
    }

    public static void Write(this IYamlStream stream, string key, ref Uri value)
    {
        stream.Serialize(ref key);
        var s = value.ToString();
        stream.Serialize(ref s);
    }
}

public static class YamlStreamExtensionsKeyValuePair
{
    public static void Write<T, K>(this IYamlStream stream, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Write(ref value);
    }

    public static void Write<T, K>(this IYamlStream stream, ref KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        if (style == DataStyle.Any)
            style = DataStyle.Normal;
        stream.Emitter.BeginSequence();
        stream.Write(value.Key, style);
        stream.Write(value.Value, style);
        stream.Emitter.EndSequence();
    }

    public static void Write<T, K>(this IYamlStream stream, string key, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key, ref value,style);
    }

    public static void Write<T, K>(this IYamlStream stream, string key, ref KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(ref value, style);
    }
}