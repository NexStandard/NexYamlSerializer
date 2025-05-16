using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMappingSecondary : FlowMapping
{
    public override WriteContext<Mapping> Write<T>(WriteContext<Mapping> context, string key, T value, DataStyle style)
    {
        // Node following a FlowMapping is prefixed with comma ", {KEY: VALUE}"
        context.WriteScalar(", ");
        return base.Write(context, key, value, style);
    }
}
