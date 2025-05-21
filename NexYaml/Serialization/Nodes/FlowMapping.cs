using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMapping : Mapping
{
    public override WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.IsRedirected)
        {
            context.WriteScalar(tag);
            context.WriteScalar(" { ");
        }
        else
        {
            context.WriteScalar("{ ");
        }
        // inside a flow, only new flows can be created, no block is allowed
        return new WriteContext<Mapping>(context.Indent, false, DataStyle.Compact, this, context.Writer);
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        // inside a flow, only new flows can be created, no block is allowed
        return new WriteContext<Sequence>(context.Indent, false, DataStyle.Compact, CommonNodes.FlowSequence, context.Writer)
            .BeginSequence(tag, DataStyle.Compact);
    }

    public override WriteContext<Mapping> Write<T>(WriteContext<Mapping> context, string key, T value, DataStyle style)
    {
        // First Node is {KEY: VALUE}
        context.WriteScalar(key + ": ");
        context.WriteType(value, style);

        // all following Nodes need a prefix
        return context with
        {
            Node = new FlowMappingSecondary()
        };
    }

    public override void End<T>(WriteContext<T> context)
    {
        context.WriteScalar(" }");
    }
}
