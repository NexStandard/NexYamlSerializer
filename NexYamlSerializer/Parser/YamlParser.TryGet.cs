using System;
using System.Collections.Generic;

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

    private readonly Dictionary<Anchor, object?> aliases = [];

    public void Reset()
    {
        aliases.Clear();
    }

    public void RegisterAnchor(Anchor anchor, object? value)
    {
        aliases[anchor] = value;
    }

    public bool TryResolveCurrentAlias<T>(ref YamlParser parser, out T? aliasValue)
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

    public bool TryGetScalarAsString(out string? value)
    {
        if (currentScalar is not null)
        {
            value = currentScalar.IsNull() ? null : currentScalar.ToString();
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
