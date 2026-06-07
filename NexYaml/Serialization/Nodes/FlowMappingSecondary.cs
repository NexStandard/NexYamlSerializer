using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMappingSecondary : FlowMapping
{
    public override WriteContext<Mapping> Begin(WriteContext<Mapping> context, string key, DataStyle style)
    {
        // Node following a FlowMapping is prefixed with comma ", {KEY: VALUE}"
        context.WriteScalar(", ");
        base.Begin(context, key, style);
        return context;
    }

}
