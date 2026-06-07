using Stride.Core;
using Stride.Input;

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
    public override WriteContext<Mapping> Begin(WriteContext<Mapping> context, string key, DataStyle style)
    {
        // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
        // "{NEWLINE}{INDENT}{KEY}: {OUTPUT FROM WriteType}"
        context.WriteScalar("\n");
        context.WriteScalar(new string(' ', context.Indent));

        // The key may contain YAML tokens, so it must be validated according to the ScalarStyle rules.
        context.WriteString(key);
        context.WriteScalar(": ");
        return context;
    }


    public override WriteContext<Mapping> End(WriteContext<Mapping> context, DataStyle style)
    {
        return context;
    }
}
