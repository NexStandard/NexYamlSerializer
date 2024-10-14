using NexVYaml.Parser;
using NexVYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml.Parser;
public static class ParserExtensions
{
    public static void DeserializeCurrent<T>(this YamlParser parser, ref T? value)
    {
        parser.Read();
        parser.DeserializeWithAlias(ref parser, ref value);
    }
    public static bool TryDeserialize<T>(this YamlParser parser, ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey)
    {
        if(key.SequenceEqual(mappingKey))
        {
            parser.Read();
            parser.DeserializeWithAlias(ref parser, ref target);
            return true;
        }
        return false;
    }
}
