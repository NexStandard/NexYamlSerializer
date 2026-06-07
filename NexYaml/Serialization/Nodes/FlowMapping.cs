using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMapping : Mapping
{
    public FlowMapping(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
        : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping BeginMapping(string tag, DataStyle style)
    {
        if (IsRedirected)
        {
            this.WriteScalar(tag);
            this.WriteScalar(" { ");
        }
        else
        {
            this.WriteScalar("{ ");
        }
        // inside a flow, only new flows can be created, no block is allowed
        return new FlowMapping(Indent, false, DataStyle.Compact, Writer);
    }

    public override Sequence BeginSequence(string tag, DataStyle style)
    {
        // inside a flow, only new flows can be created, no block is allowed
        return new FlowSequence(Indent, IsRedirected, StyleScope, Writer)
            .BeginSequence(tag, DataStyle.Compact);
    }
    public override void End()
    {
        this.WriteScalar(" }");
    }

    public override Mapping Begin(Mapping context, string key, DataStyle style)
    {
        // First Node is {KEY: VALUE}
        this.WriteScalar(key);
        this.WriteScalar(": ");
        return context;
    }
    public override Mapping End(Mapping context, DataStyle style)
    {
        // all following Nodes need a prefix
        return new FlowMappingSecondary(Indent, IsRedirected, style, Writer);
    }


}
