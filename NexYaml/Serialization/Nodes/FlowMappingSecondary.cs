using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMappingSecondary : FlowMapping
{
    public FlowMappingSecondary(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
        : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping Begin(Mapping context, string key, DataStyle style)
    {
        // Node following a FlowMapping is prefixed with comma ", {KEY: VALUE}"
        this.WriteScalar(", ");
        base.Begin(this,key, style);
        return this;
    }

}
