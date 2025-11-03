using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockMapping : Mapping
{
    public override WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return CommonNodes.FlowMapping.BeginMapping(context, tag, DataStyle.Compact);
        }
        if (context.IsRedirected)
        {
            context.WriteScalar(tag);
        }
        return new WriteContext<Mapping>(context.Indent + 2, false, DataStyle.Normal, this, context.Writer);
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return CommonNodes.FlowSequence.BeginSequence(context, tag, DataStyle.Compact);
        }
        return CommonNodes.BlockSequence.BeginSequence(context, tag, DataStyle.Normal);
    }

    public override WriteContext<Mapping> Write<T>(WriteContext<Mapping> context, string key, T value, DataStyle style)
    {
        // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
        // "{NEWLINE}{INDENT}{KEY}: {OUTPUT FROM WriteType}"
        context.WriteScalar("\n");
        context.WriteScalar(new string(' ', context.Indent));

        // The key may contain YAML tokens, so it must be validated according to the ScalarStyle rules.
        context.WriteString(key);
        context.WriteScalar(": ");
        context.WriteType(value, style);
        return context;
    }

    public override WriteContext<Mapping> Write(WriteContext<Mapping> context, string key, ReadOnlySpan<char> value, DataStyle style)
    {
        // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
        // "{NEWLINE}{INDENT}{KEY}: {OUTPUT FROM WriteType}"
        context.WriteScalar("\n" + new string(' ', context.Indent));

        // The key may contain YAML tokens, so it must be validated according to the ScalarStyle rules.
        context.WriteString(key);
        context.WriteScalar(": ");
        context.WriteScalar(value);
        return context;
    }
}
