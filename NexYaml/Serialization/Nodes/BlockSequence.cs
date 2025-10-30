using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequence : Sequence
{
    public override WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return CommonNodes.FlowMapping.BeginMapping(context, tag, DataStyle.Compact);
        }

        //  When no tag is provided, BlockMapping would introduce an extra faulty newline.
        if (context.IsRedirected)
        {
            // - {TAG}\n
            //   {KEY} : {VALUE}
            return CommonNodes.BlockMapping.BeginMapping(context, tag, DataStyle.Normal);
        }
        else
        {
            // - {KEY} : {VALUE}
            return CommonNodes.BlockSequenceMapping.BeginMapping(context, tag, DataStyle.Normal);
        }
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return CommonNodes.FlowSequence.BeginSequence(context, tag, DataStyle.Compact);
        }
        if (context.IsRedirected)
        {
            context.WriteScalar(tag);
        }
        return new WriteContext<Sequence>(Math.Max(0, context.Indent) + 2, false, DataStyle.Normal, this, context.Writer);
    }

    public override WriteContext<Sequence> Write<T>(WriteContext<Sequence> context, T value, DataStyle style)
    {
        // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
        // "{NEWLINE}{INDENT}- {OUTPUT FROM WriteType}"
        // Special rule:
        // - The sequence identifier ("- ") does NOT use increased indentation.
        // - The indent can NOT be below 0
        // - Only the subsequent nodes follow deeper indentation levels.
        context.WriteScalar("\n");
        context.WriteScalar(new string(' ', Math.Max(context.Indent - 2, 0)));
        context.WriteScalar("- ");
        context.WriteType(value, style);
        return context;
    }
}
