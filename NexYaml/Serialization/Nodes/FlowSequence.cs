using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequence : Sequence
{
    public override WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        return CommonNodes.FlowMapping.BeginMapping(context, tag, DataStyle.Compact);
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        context.WriteScalar(tag + " [ ");
        return new WriteContext<Sequence>(context.Indent, false, DataStyle.Compact, this, context.Writer);
    }

    public override WriteContext<Sequence> Write<T>(WriteContext<Sequence> context, T value, DataStyle style)
    {
        context.WriteType(value, style);
        return context with
        {
            Node = new FlowSequenceSecondary(),
        };
    }
    public override void End<T>(WriteContext<T> context)
    {
        context.WriteScalar(" ]");
    }
}
