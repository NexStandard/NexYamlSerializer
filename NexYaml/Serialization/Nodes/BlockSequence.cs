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
        if (context.IsRedirected)
        {
            return CommonNodes.BlockMapping.BeginMapping(context, tag, DataStyle.Normal);
        }
        else
        {
            return CommonNodes.BlockSequenceMapping.BeginMapping(context, tag, DataStyle.Normal);
        }
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return new FlowSequence().BeginSequence(context, tag, DataStyle.Compact);
        }
        if (context.IsRedirected)
        {
            context.WriteScalar(tag);
        }
        return new WriteContext<Sequence>(context.Indent + 2, false, DataStyle.Normal, this, context.Writer);
    }

    public override WriteContext<Sequence> Write<T>(WriteContext<Sequence> context, T value, DataStyle style)
    {
        context.WriteScalar("\n" + new string(' ', Math.Max(context.Indent - 2, 0)) + "- ");
        context.WriteType(value, style);
        return context;
    }
}
