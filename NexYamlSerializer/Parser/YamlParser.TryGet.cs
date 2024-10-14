#nullable enable
using NexVYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NexVYaml.Parser;

public partial class YamlParser
{
    public void SkipRead()
    {
        Read();
        SkipCurrentNode();
    }
    public bool IsNullScalar()
    {
        return CurrentEventType == ParseEventType.Scalar &&
               (currentScalar == null ||
                currentScalar.IsNull());
    }
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

    public void DeserializeWithAlias<T>(YamlSerializer<T> innerFormatter, ref YamlParser parser, ref T value)
    {
        if (TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            value = aliasValue!;
            return;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        innerFormatter.Deserialize(parser, ref value);

        if (withAnchor)
        {
            RegisterAnchor(anchor, value);
        }
        return;
    }
    public void DeserializeWithAlias<T>(ref YamlParser parser, ref T? value)
    {
        if (typeof(T).IsArray)
        {
            var t = typeof(T).GetElementType()!;
            object? val = value;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Deserialize(parser, ref val);
            value = (T)val!;
            return;
        }
        var type = typeof(T);
        if (IsNullable(type, out var underlyingType))
        {
            var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);
            // TODO : Nullable makes sense?
            var f = (YamlSerializer<T?>?)Activator.CreateInstance(genericFilledFormatter)!;
            f.Deserialize(parser, ref value);
            return;
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            parser.TryGetCurrentTag(out var tag);
            YamlSerializer? formatter;
            if (tag == null)
            {
                var formatt = resolver.GetGenericFormatter<T>();
                if (formatt == null)
                {
                    value = default;
                    return;
                }

                else
                {
                    DeserializeWithAlias(formatt, ref parser, ref value);
                    return;
                }
            }
            else
            {
                Type alias;
                // TODO: Problem is that !!null etc gets consumed as Tag on collections instead of a scalar value
                if (parser.IsNullScalar())
                {
                    parser.Read();
                    value = default;
                    return;
                }
                alias = Resolver.GetAliasType(tag.Handle);
                formatter = Resolver.GetFormatter(alias);
                formatter ??= Resolver.GetFormatter(alias, type);
            }
            if (formatter == null)
            {
                value = default;
                return;
            }
            var valueObject = (object)value;
            formatter.Deserialize(parser, ref valueObject);
            value = (T?)valueObject;
        }
        else
        {
            Resolver.GetFormatter<T>().Deserialize(parser, ref value!);
        }
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
                        throw new YamlException($"The alias value is not a type of {typeof(T).Name}");
                }
            }
            throw new YamlException($"Could not found an alias value of anchor: {anchor}");
        }

        aliasValue = default;
        return false;
    }
    public bool TryGetScalarAsSpan(out ReadOnlySpan<byte> span)
    {
        if (currentScalar is null)
        {
            span = default;
            return false;
        }
        span = currentScalar.AsSpan();
        return true;
    }

    public string? ReadScalarAsString()
    {
        var result = currentScalar?.ToString();
        ReadWithVerify(ParseEventType.Scalar);
        return result;
    }

    public bool TryGetScalarAsString(out string? value)
    {
        if (currentScalar is { } scalar)
        {
            value = scalar.IsNull() ? null : scalar.ToString();
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGetCurrentTag(out Tag tag)
    {
        if (currentTag != null)
        {
            tag = currentTag;
            return true;
        }
        tag = default!;
        return false;
    }

    public bool TryGetCurrentAnchor(out Anchor anchor)
    {
        if (currentAnchor != null)
        {
            anchor = currentAnchor;
            return true;
        }
        anchor = default!;
        return false;
    }
}
