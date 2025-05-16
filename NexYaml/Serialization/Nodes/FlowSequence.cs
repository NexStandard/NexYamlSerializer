using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequence : Sequence
{
    public override WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        // inside a flow, only new flows can be created, no block is allowed
        return CommonNodes.FlowMapping.BeginMapping(context, tag, DataStyle.Compact);
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.IsRedirected)
        {
            context.WriteScalar(tag);
        }
        context.WriteScalar(" [ ");

        // inside a flow, only new flows can be created, no block is allowed
        return new WriteContext<Sequence>(context.Indent, false, DataStyle.Compact, this, context.Writer);
    }

    public override WriteContext<Sequence> Write<T>(WriteContext<Sequence> context, T value, DataStyle style)
    {
        // First Node is {VALUE}
        context.WriteType(value, style);

        // all following Nodes need a prefix
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
