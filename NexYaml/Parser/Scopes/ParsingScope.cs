using System;
using System.Collections.Generic;
using System.Text;
using NexYaml.Core;
using NexYaml.Core.Parser;
using NexYaml.Serialization;
using static Stride.Graphics.Buffer;

namespace NexYaml.Parser.Scopes;

public class ParsingScope
{

    public static Scope NewScalar(ReadOnlySpan<char> value, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.Scalar,
            ScalarValue = value.ToString()
        };
    }
    public static Scope NewLazyScalar(int valueIndex, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.LazyScalar,
        };
    }
    public static Scope NewNullScalar()
    {
        return new Scope()
        {
            Kind = ScopeKind.NullScalar,
        };
    }
    public static Scope NewFlowSequence(ReadOnlySpan<char> value, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.FlowSequence,
            sequenceStrategy = new FlowSequenceStrategy(ScopeUtils.NewSplitItems(value.ToString().Substring(1, value.Length - 2).Trim()))
        };
    }
    public static Scope NewBlockSequence(int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.BlockSequence,
            sequenceStrategy = new BlockSequenceStrategy()
        };
    }
    public static Scope NewFlowMapping(ReadOnlySpan<char> value, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.FlowMapping,
            ScalarValue = value.ToString(),
            mapStrategy = new FlowMapStrategy(ScopeUtils.NewSplitItems(value.ToString().Substring(1, value.Length - 2).Trim()))
        };
    }
    public static Scope NewBlockMapping(int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.BlockMapping,
            mapStrategy = new BlockMapStrategy()
        };
    }
    public static Scope NewPrefixedBlockMapping(ReadOnlySpan<char> value, string prefix, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.PrefixedBlockMapping,
            ScalarValue = value.ToString(),
            mapStrategy = new PrefixedBlockMapStrategy(value.ToString(),prefix)
        };
    }

}
