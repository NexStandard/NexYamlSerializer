using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequenceMapping : BlockMapping
{
    public override WriteContext<Mapping> Write<T>(WriteContext<Mapping> context, string key, T value, DataStyle style)
    {
        // If the tag is not present on Sequence element:
        //    {INDENT}- {KEY}: {VALUE}
        context.WriteString(key);
        context.WriteScalar(" : ");

        // continue with standard BlockMapping, -2 just negates the + 2 of the upcomming BeginMapping
        var x = context with
        {
            Node = new BlockMapping(),
            Indent = context.Indent - 2,
        };
        x.WriteType(value, style);

        return x;
    }
}
