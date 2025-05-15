using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMappingSecondary : FlowMapping
{
    public override WriteContext<Mapping> Write<T>(string key, T value, WriteContext<Mapping> context, DataStyle style)
    {
        context.WriteScalar(", ");
        return base.Write(key, value, context, style);
    }
}
