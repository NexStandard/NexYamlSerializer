using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequenceMapping : BlockMapping
{
    public override WriteContext<Mapping> Write<T>(string key, T value, WriteContext<Mapping> context, DataStyle style)
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
