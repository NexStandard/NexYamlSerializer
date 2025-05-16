using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequenceSecondary : FlowSequence
{
    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        context.WriteScalar(tag + " [ ");
        return new WriteContext<Sequence>(context.Indent, false, DataStyle.Compact, CommonNodes.FlowSequence, context.Writer);
    }
    public override WriteContext<Sequence> Write<T>(T value, WriteContext<Sequence> context, DataStyle style)
    {
        context.WriteScalar(", ");
        return base.Write(value, context, style);
    }
}
