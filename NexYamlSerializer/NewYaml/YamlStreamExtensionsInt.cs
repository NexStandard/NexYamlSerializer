using NexVYaml.Serialization;
using NexYamlSerializer;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml;

public static class YamlStreamExtensionsInt
{
    public static void Write(this ISerializationWriter stream, int value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, int? value)
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

    public static void Write(this ISerializationWriter stream, ref int value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ref int? value)
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

    public static void Write(this ISerializationWriter stream, string key, int? value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, int value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref int value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref int? value)
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
    public static void Write(this ISerializationWriter stream, double value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, double? value)
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

    public static void Write(this ISerializationWriter stream, ref double value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ref double? value)
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

    public static void Write(this ISerializationWriter stream, string key, double? value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, double value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref double value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref double? value)
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
    public static void Write(this ISerializationWriter stream, ulong value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ulong? value)
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

    public static void Write(this ISerializationWriter stream, ref ulong value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ref ulong? value)
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

    public static void Write(this ISerializationWriter stream, string key, ulong? value)
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

    public static void Write(this ISerializationWriter stream, string key, ulong value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref ulong value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref ulong? value)
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
    public static void Write(this ISerializationWriter stream, float value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, float? value)
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

    public static void Write(this ISerializationWriter stream, ref float value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ref float? value)
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

    public static void Write(this ISerializationWriter stream, string key, float? value)
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

    public static void Write(this ISerializationWriter stream, string key, float value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref float value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref float? value)
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
    public static void Write(this ISerializationWriter stream, bool value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, bool? value)
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

    public static void Write(this ISerializationWriter stream, ref bool value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ref bool? value)
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

    public static void Write(this ISerializationWriter stream, string key, bool? value)
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

    public static void Write(this ISerializationWriter stream, string key, bool value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref bool value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref bool? value)
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
    public static void Write(this ISerializationWriter stream, uint value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, uint? value)
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

    public static void Write(this ISerializationWriter stream, ref uint value)
    {
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, ref uint? value)
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

    public static void Write(this ISerializationWriter stream, string key, uint? value)
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

    public static void Write(this ISerializationWriter stream, string key, uint value)
    {
        stream.Write(key, ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref uint value)
    {
        stream.Serialize(ref key);
        stream.Serialize(ref value);
    }

    public static void Write(this ISerializationWriter stream, string key, ref uint? value)
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
    public static void Write(this ISerializationWriter stream, Uri value)
    {
        var s = value.ToString();
        stream.Serialize(ref s);
    }

    public static void Write(this ISerializationWriter stream, ref Uri value)
    {
        var s = value.ToString();
        stream.Serialize(ref s);
    }

    public static void Write(this ISerializationWriter stream, string key, Uri value)
    {
        var s = value.ToString();
        stream.Write(key, ref s);
    }

    public static void Write(this ISerializationWriter stream, string key, ref Uri value)
    {
        stream.Serialize(ref key);
        var s = value.ToString();
        stream.Serialize(ref s);
    }
}

public static class YamlStreamExtensionsKeyValuePair
{
    public static void Write<T, K>(this ISerializationWriter stream, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Write(ref value);
    }

    public static void Write<T, K>(this ISerializationWriter stream, ref KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        if (style == DataStyle.Any)
            style = DataStyle.Normal;
        stream.Emitter.BeginSequence();
        stream.Write(value.Key, style);
        stream.Write(value.Value, style);
        stream.Emitter.EndSequence();
    }

    public static void Write<T, K>(this ISerializationWriter stream, string key, KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Write(key, ref value,style);
    }

    public static void Write<T, K>(this ISerializationWriter stream, string key, ref KeyValuePair<T, K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(ref value, style);
    }
}
public static class YamlStreamExtensionsList
{
    public static void Write<T>(this ISerializationWriter stream, List<T> value, DataStyle style = DataStyle.Any)
    {
        if(value is null)
        {
            stream.WriteNull();
            return;
        }
        stream.Emitter.BeginSequence(style);
        foreach (var x in value)
        {
            stream.Write(x, style);
        }

        stream.Emitter.EndSequence();
    }

    public static void Write<T>(this ISerializationWriter stream, string key, List<T> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
}
public static class YamlStreamExtensionsDictionary
{
    public static void Write<TKey,TValue>(this ISerializationWriter stream, Dictionary<TKey,TValue> value, DataStyle style = DataStyle.Any)
    {
        if(value is null)
        {
            stream.WriteNull();
            return;
        }

        YamlSerializer<TKey> keyFormatter = null;
        YamlSerializer<TValue> valueFormatter = null;
        if (FormatterExtensions.IsPrimtiveType(typeof(TKey)))
        {
            keyFormatter = NewSerializerRegistry.Instance.GetFormatter<TKey>();
        }
        if (FormatterExtensions.IsPrimtiveType(typeof(TValue)))
            valueFormatter = NewSerializerRegistry.Instance.GetFormatter<TValue>();

        if (keyFormatter == null)
        {
            stream.Emitter.BeginSequence();
            if (value.Count > 0)
            {
                var elementFormatter = new KeyValuePairFormatter<TKey, TValue>();
                foreach (var x in value)
                {
                    elementFormatter.Serialize(ref stream, x);
                }
            }
            stream.Emitter.EndSequence();
        }
        else if (valueFormatter == null)
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    stream.Write(x.Value);
                }
            }
            stream.Emitter.EndMapping();
        }
        else
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, style);
                    valueFormatter.Serialize(ref stream, x.Value, style);
                }
            }
            stream.Emitter.EndMapping();
        }
    }

    public static void Write<T,K>(this ISerializationWriter stream, string key, Dictionary<T,K> value, DataStyle style = DataStyle.Any)
    {
        stream.Serialize(ref key);
        stream.Write(value, style);
    }
}