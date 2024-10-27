using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
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
    public static void ReadMapping(this IYamlReader stream, Action action)
    {
        stream.ReadWithVerify(ParseEventType.MappingStart);
        while(stream.HasKeyMapping)
        {
            action();
        }
        stream.ReadWithVerify(ParseEventType.MappingEnd);
    }
    public static void ReadSequence(this IYamlReader stream, Action action)
    {
        stream.ReadWithVerify(ParseEventType.SequenceStart);
        while (stream.HasSequence)
        {
            action();
        }
        stream.ReadWithVerify(ParseEventType.SequenceEnd);
    }
}
