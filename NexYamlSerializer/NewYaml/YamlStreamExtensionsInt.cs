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
        stream.Write(key, ref value);
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
        stream.Write(key, ref value);
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
        stream.Write(key, ref value);
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
