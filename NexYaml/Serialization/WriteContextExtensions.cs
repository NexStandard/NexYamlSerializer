using NexYaml.Core;
using System.IO;
using Stride.Core;

namespace NexYaml.Serialization;

public static class WriteContextExtensions
{
    public static void WriteScalar<T>(this WriteContext<T> context, ReadOnlySpan<char> text) where T : Node
    {
        context.Writer.Write(text);
    }

    public static WriteContext<Mapping> BeginMapping<T>(this WriteContext<T> mapping, string tag, DataStyle style)
        where T : Node
    {
        return mapping.Node.BeginMapping(mapping,tag, style);
    }

    public static WriteContext<Sequence> BeginSequence<T>(this WriteContext<T> mapping, string tag, DataStyle style)
        where T : Node
    {
        return mapping.Node.BeginSequence(mapping,tag, style);
    }

    public static void WriteType<T, X>(this WriteContext<T> context, X value, DataStyle style)
        where T : Node
    {
        context.Writer.WriteType(context, value, style);
    }

    public static void End<T,X>(this WriteContext<T> context, in WriteContext<X> current)
        where T : Node
        where X : Node
    {
        context.Node.End(current);
    }

    public static void WriteEmptyMapping<T>(this WriteContext<T> context, string tag)
        where T : Node
    {
        context.WriteScalar(tag + " { }");
    }

    public static void WriteEmptySequence<T>(this WriteContext<T> context, string tag)
        where T : Node
    {
        context.WriteScalar(tag + " [ ]");
    }
    public static WriteContext<Mapping> Write<T>(this WriteContext<Mapping> mapping, string key ,T value, DataStyle style = DataStyle.Any)
    {
        return mapping.Node.Write(mapping, key, value,style);
    }

    public static WriteContext<Sequence> Write<T>(this WriteContext<Sequence> sequence, T value, DataStyle style = DataStyle.Any)
    {
        return sequence.Node.Write(sequence, value, style);
    }
    public static void WriteString<X>(this WriteContext<X> context, string value, DataStyle style = DataStyle.Compact)
        where X : Node
    {
        context.Writer.WriteString(context,value, style);
    }
}
