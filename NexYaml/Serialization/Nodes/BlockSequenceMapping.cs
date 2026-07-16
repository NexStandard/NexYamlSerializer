using NexYaml.Core.Serialization.Nodes;
using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequenceMapping : BlockMapping
{
    public BlockSequenceMapping(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
        : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping BeginMapping(string tag, DataStyle style)
    {
        // TODO: how can this even happen, missing test for these ifs
        if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return new BlockMapping(Indent, IsRedirected, StyleScope, Writer).BeginMapping(tag, DataStyle.Compact);
        }
        if (IsRedirected)
        {
            WriteScalar(tag);
        }
        return new BlockSequenceMapping(Indent + 2, false, DataStyle.Normal, Writer);
    }
    public override Mapping WriteKey(Mapping context, ReadOnlySpan<char> key, DataStyle style)
    {
        // If the tag is not present on Sequence element:
        //    {INDENT}- {KEY}: {VALUE}
        context.WriteScalar(key);
        context.WriteScalar(": ");

        return new BlockMapping(context.Indent - 2, IsRedirected, StyleScope, Writer);
    }
}
