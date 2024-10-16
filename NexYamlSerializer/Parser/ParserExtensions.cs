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

    public static bool IsNullable(this YamlParser parser, Type value, [MaybeNullWhen(false)] out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }
}
