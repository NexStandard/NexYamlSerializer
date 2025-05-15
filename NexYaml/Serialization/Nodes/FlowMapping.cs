using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMapping : Mapping
{
    public override WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        context.WriteScalar(tag + " { ");
        return new WriteContext<Mapping>(context.Indent, false, DataStyle.Compact, this, context.Writer);
    }

    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        return new WriteContext<Sequence>(context.Indent, false, DataStyle.Compact, new FlowSequence(), context.Writer)
            .BeginSequence(tag,DataStyle.Compact);
    }

    public override WriteContext<Mapping> Write<T>(string key, T value, WriteContext<Mapping> context, DataStyle style)
    {
        context.WriteScalar(key + ": ");
        context.WriteType(value, style);
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
