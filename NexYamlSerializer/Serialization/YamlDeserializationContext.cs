#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using NexYamlSerializer.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NexVYaml.Serialization;

public class YamlDeserializationContext(YamlSerializerOptions options)
{
    public IYamlFormatterResolver Resolver { get; } = options.Resolver;
    public bool SecureMode { get; set; } = options.SecureMode;
    readonly Dictionary<Anchor, object?> aliases = [];

    public void Reset()
    {
        aliases.Clear();
    }
    public static readonly Type NullableFormatter = typeof(NullableFormatter<>);
    public static bool IsNullable(Type value, [MaybeNullWhen(false)] out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }

    public T[]? DeserializeArray<T>(ref YamlParser parser)
    {
        return new ArrayFormatter<T>().Deserialize(ref parser, this);
    }
    public void DeserializeWithAlias<T>(YamlSerializer<T> innerFormatter, ref YamlParser parser, ref T value)
    {
        if (TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            value = aliasValue!;
            return;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        var result = innerFormatter.Deserialize(ref parser, this);

        if (withAnchor)
        {
            RegisterAnchor(anchor, result);
        }
        value = result!;
        return;
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
public static class DeserializeExtensions
{
    public static void DeserializeWithAlias<T>(this YamlDeserializationContext context,ref YamlParser parser, ref T? value)
    {
        var type = typeof(T);
        if (YamlDeserializationContext.IsNullable(type, out var underlyingType))
        {
            var genericFilledFormatter = YamlDeserializationContext.NullableFormatter.MakeGenericType(underlyingType);
            // TODO : Nullable makes sense?
            var f = (YamlSerializer<T?>?)Activator.CreateInstance(genericFilledFormatter)!;
            value = (f).Deserialize(ref parser, context);
            return;
        }
        else
        if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            if (context.SecureMode)
            {
                context.DeserializeWithAlias(context.Resolver.GetFormatter<T>(), ref parser, ref value);
                return;
            }

            parser.TryGetCurrentTag(out var tag);
            YamlSerializer? formatter;
            if (tag == null)
            {
                var formatt = context.Resolver.GetGenericFormatter<T>();
                if (formatt == null)
                {
                    value = default;
                    return;
                }

                else
                {
                    context.DeserializeWithAlias(formatt, ref parser, ref value);
                    return;
                }
            }
            else
            {
                Type alias;
                // TODO: Problem is that !!null etc gets consumed as Tag instead of a scalar value
                if (parser.IsNullScalar())
                {
                    parser.Read();
                    value = default;
                    return;
                }
                alias = context.Resolver.GetAliasType(tag.Handle);
                formatter = context.Resolver.GetFormatter(alias);
                if (formatter is null)
                {
                    formatter = context.Resolver.GetFormatter(alias, type);
                }
            }
            if (formatter == null)
            {
                value = default;
                return;
            }
            value = (T?)formatter.IndirectDeserialize(ref parser, context);
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            // var method = formatter.GetType().GetMethod("Deserialize");
            // return (T)method.Invoke(formatter, new object[] { parser, this });
        }
        else
        {
            value = context.Resolver.GetFormatter<T>().Deserialize(ref parser, context);
        }
    }
    public static void DeserializeWithAlias(this YamlDeserializationContext context, ref YamlParser parser, ref int value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = result;
    }
}