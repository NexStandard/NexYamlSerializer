#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer.Serialization.Formatters;
using System;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class YamlDeserializationContext
{
    public IYamlFormatterResolver Resolver { get; }
    public bool SecureMode { get; set; } = false;
    readonly Dictionary<Anchor, object?> aliases = new();

    public YamlDeserializationContext(YamlSerializerOptions options)
    {
        Resolver = options.Resolver;
        SecureMode = options.SecureMode;
    }

    public void Reset()
    {
        aliases.Clear();
    }
    static Type NullableFormatter = typeof(NullableFormatter<>);
    public static bool IsNullable(Type value, out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }
    public T? DeserializeWithAlias<T>(ref YamlParser parser)
    {
        var type = typeof(T);
        if (IsNullable(type, out var underlyingType))
        {
            var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);

            return ((YamlSerializer<T>)Activator.CreateInstance(genericFilledFormatter, args: Resolver.GetFormatter(underlyingType))).Deserialize(ref parser, this);
        }
        else
        if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            if (SecureMode)
            {
                return DeserializeWithAlias(Resolver.GetFormatter<T>(), ref parser);
            }

            parser.TryGetCurrentTag(out var tag);
            YamlSerializer formatter;
            if (tag == null)
            {
                YamlSerializer<T> formatt = Resolver.GetGenericFormatter<T>();
                if (formatt == null)
                    return new EmptyFormatter<T>().Deserialize(ref parser, this);
                else
                    return DeserializeWithAlias(formatt, ref parser);
            }
            else
            {
                Type alias;
                // enable aliasing on redirection
                alias = Resolver.GetAliasType(tag.Handle);
                formatter = Resolver.GetFormatter(alias);
                if (formatter == null)
                {
                    formatter = Resolver.GetFormatter(alias, type);
                }
            }
            if (formatter == null)
                return new EmptyFormatter<T>().Deserialize(ref parser, this);
            return (T?)formatter.IndirectDeserialize(ref parser, this);
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            // var method = formatter.GetType().GetMethod("Deserialize");
            // return (T)method.Invoke(formatter, new object[] { parser, this });
        }
        else
        {
            return Resolver.GetFormatter<T>().Deserialize(ref parser, this);
        }
    }
    public T[]? DeserializeArray<T>(ref YamlParser parser)
    {
        return new ArrayFormatter<T>().Deserialize(ref parser, this);
    }
    public T DeserializeWithAlias<T>(YamlSerializer<T> innerFormatter, ref YamlParser parser)
    {
        if (TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            return aliasValue!;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        var result = innerFormatter.Deserialize(ref parser, this);

        if (withAnchor)
        {
            RegisterAnchor(anchor, result);
        }
        return result;
    }

    void RegisterAnchor(Anchor anchor, object? value)
    {
        aliases[anchor] = value;
    }

    bool TryResolveCurrentAlias<T>(ref YamlParser parser, out T? aliasValue)
    {
        if (parser.CurrentEventType != ParseEventType.Alias)
        {
            aliasValue = default;
            return false;
        }

        if (parser.TryGetCurrentAnchor(out var anchor))
        {
            parser.Read();
            if (aliases.TryGetValue(anchor, out var obj))
            {
                switch (obj)
                {
                    case null:
                        aliasValue = default;
                        return true;
                    case T value:
                        aliasValue = value;
                        return true;
                    default:
                        throw new YamlSerializerException($"The alias value is not a type of {typeof(T).Name}");
                }
            }
            throw new YamlSerializerException($"Could not found an alias value of anchor: {anchor}");
        }

        aliasValue = default;
        return false;
    }
}
