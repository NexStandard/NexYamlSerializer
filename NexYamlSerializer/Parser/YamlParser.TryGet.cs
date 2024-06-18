#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace NexVYaml.Parser;

public partial class YamlParser
{
    public bool IsNullScalar()
    {
        return CurrentEventType == ParseEventType.Scalar &&
               (currentScalar == null ||
                currentScalar.IsNull());
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
