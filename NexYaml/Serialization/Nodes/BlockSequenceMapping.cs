using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequenceMapping : BlockMapping
{
    public override WriteContext<Mapping> Write<T>(WriteContext<Mapping> context, string key, T value, DataStyle style)
    {
        context.WriteString(key);
        context.WriteScalar(" : ");
        var x = context with
        {
            Node = new BlockMapping(),
            Indent = context.Indent -2,
        };
        x.WriteType(value, style);

        return x;
    }
}
