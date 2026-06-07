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
    public override void End<T>(WriteContext<T> context)
    {
        context.WriteScalar(" }");
    }

    public override WriteContext<Mapping> Begin(WriteContext<Mapping> context, string key, DataStyle style)
    {
        // First Node is {KEY: VALUE}
        context.WriteScalar(key);
        context.WriteScalar(": ");
        return context;
    }
    public override WriteContext<Mapping> End(WriteContext<Mapping> context, DataStyle style)
    {
        // all following Nodes need a prefix
        return context with
        {
            Node = CommonNodes.FlowMappingSecondary
        };
    }


}
