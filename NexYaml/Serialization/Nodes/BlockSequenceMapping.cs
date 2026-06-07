using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequenceMapping : BlockMapping
{
    public override WriteContext<Mapping> Begin(WriteContext<Mapping> context, string key, DataStyle style)
    {
        // If the tag is not present on Sequence element:
        //    {INDENT}- {KEY}: {VALUE}
        context.WriteString(key);
        context.WriteScalar(": ");

        return context with
        {
            Node = new BlockMapping(),
            Indent = context.Indent - 2,
        };
    }
    public override WriteContext<Mapping> End(WriteContext<Mapping> context, DataStyle style)
    {
        return context;
    }
}
