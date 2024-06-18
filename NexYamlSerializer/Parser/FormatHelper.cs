using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Parser;
internal class FormatHelper
{
    public static bool TryDetectHex(ReadOnlySpan<byte> span, out ReadOnlySpan<byte> slice)
    {
        if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith(YamlCodes.HexPrefix))
        {
            slice = span[YamlCodes.HexPrefix.Length..];
            return true;
        }

        slice = default;
        return false;
    }

    public static bool TryDetectHexNegative(ReadOnlySpan<byte> span, out ReadOnlySpan<byte> slice)
    {
        if (span.Length > YamlCodes.HexPrefixNegative.Length &&
            span.StartsWith(YamlCodes.HexPrefixNegative))
        {
            slice = span[YamlCodes.HexPrefixNegative.Length..];
            return true;
        }

        slice = default;
        return false;
    }
}
