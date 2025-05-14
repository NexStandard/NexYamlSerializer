using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYaml.Serialization;
using Stride.Core;

namespace ConsoleApp2;
abstract class Node
{
    public abstract Context<Mapping> BeginMapping<T>(Context<T> context, string tag, DataStyle style)
        where T : Node;
    public abstract Context<Sequence> BeginSequence<T>(Context<T> context, string tag, DataStyle style)
        where T : Node;
    public virtual void End<T>(in Context<T> context) where T : Node
    {
        // standard do nothing
    }
    public virtual void WriteScalar<T>(ReadOnlySpan<char> text, in Context<T> context) where T : Node
    {
        Console.Write(text.ToString());
    }
}
abstract class Mapping : Node
{
    public abstract Context<Mapping> Write<T>(string key,T value, Context<Mapping> context);
}
abstract class Sequence : Node
{
    public abstract Context<Sequence> Write<T>(T value, Context<Sequence> context);
}

internal record Context<T>(int Indent, bool IsRedirected, IYamlWriter Stream, DataStyle StyleScope, T Node) where T : Node;

abstract class Serializer<T>
{
    public abstract void Write<X>(T value, Context<X> context)
        where X : Node;
}

static class Extensions
{
    public static Context<Mapping> BeginMapping<T>(this Context<T> mapping, string tag, DataStyle style)
        where T : Node
    {
        return mapping.Node.BeginMapping(mapping,tag, style);
    }

    public static Context<Sequence> BeginSequence<T>(this Context<T> mapping, string tag, DataStyle style)
        where T : Node
    {
        return mapping.Node.BeginSequence(mapping,tag, style);
    }


    public static void End<T,X>(this Context<T> context, in Context<X> current)
        where T : Node
        where X : Node
    {
        context.Node.End(current);
    }

    public static void WriteScalar<T>(this Context<T> mapping, ReadOnlySpan<char> text)
        where T : Node
    {
        mapping.Node.WriteScalar(text, mapping);
    }
    public static Context<Mapping> Write<T>(this Context<Mapping> mapping, string key ,T value)
    {
        return mapping.Node.Write(key, value, mapping);
    }
    public static Context<Sequence> Write<T>(this Context<Sequence> mapping, T value)
    {
        return mapping.Node.Write(value, mapping);
    }
}

class StringSerializer : Serializer<string>
{
    public override void Write<X>(string value, Context<X> context)
    {
        context.WriteScalar(value);
    }
}
class TestSerializer : Serializer<TestS>
{
    public override void Write<X>(TestS value, Context<X> context)
    {
        context.BeginMapping("!Test", DataStyle.Normal)
            .Write("Help", "me")
            .End(context);
    }
}
class ListSerializer : Serializer<List<TestS>>
{
    public override void Write<X>(List<TestS> value, Context<X> context)
    {
        var tempContext = context.BeginSequence("!List", context.StyleScope);
        foreach(var x in value)
        {
            tempContext = tempContext.Write(x);
        }
        tempContext.End(context);
    }
}
class TestS
{
    public string X;
    public string Y;
}