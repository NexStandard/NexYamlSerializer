using NexVYaml.Parser;
using NexVYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml.Parser;
public static class ParserExtensions
{
    public static void ReadCurrent<T>(this IYamlReader parser, ref T? value)
    {
        parser.Read();
        parser.Read(ref value);
    }
    public static bool TryRead<T>(this IYamlReader parser, ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey)
    {
        if(key.SequenceEqual(mappingKey))
        {
            parser.Read();
            parser.Read(ref target);
            return true;
        }
        return false;
    }
    public static bool IsNullable(this YamlParser parser, Type value, [MaybeNullWhen(false)] out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }
}
