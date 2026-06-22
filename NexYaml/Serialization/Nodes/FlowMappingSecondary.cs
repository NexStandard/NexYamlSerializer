using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMappingSecondary : FlowMapping
{
    public FlowMappingSecondary(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
        : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping WriteKey(Mapping context, ReadOnlySpan<char> key, DataStyle style)
    {
        // Node following a FlowMapping is prefixed with comma ", {KEY: VALUE}"
        WriteScalar(", ");
        base.WriteKey(this,key, style);
        return this;
    }

}
