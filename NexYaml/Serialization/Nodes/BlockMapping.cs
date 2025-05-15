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

    public override WriteContext<Mapping> Write<T>(string key, T value, WriteContext<Mapping> context, DataStyle style)
    {
        context.WriteScalar("\n"+ new string(' ', context.Indent));
        context.WriteString($"{key}");
        context.WriteScalar(": ");
        context.WriteType(value, style);
        return context;
    }
}