using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequenceSecondary : FlowSequence
{
    public override WriteContext<Sequence> Write<T>(T value, WriteContext<Sequence> context, DataStyle style)
    {
        context.WriteScalar(", ");
        return base.Write(value, context, style);
    }
}
