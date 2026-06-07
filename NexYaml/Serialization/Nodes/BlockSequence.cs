using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class BlockSequence : Sequence
{
    public BlockSequence(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
        : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping BeginMapping(string tag, DataStyle style)
    {
        if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return new FlowMapping(Indent, IsRedirected, DataStyle.Compact, Writer).BeginMapping(tag, DataStyle.Compact);
        }

        //  When no tag is provided, BlockMapping would introduce an extra faulty newline.
        if (IsRedirected)
        {
            // - {TAG}\n
            //   {KEY} : {VALUE}
            return new BlockMapping(Indent, IsRedirected, DataStyle.Compact, Writer).BeginMapping(tag, DataStyle.Normal);
        }
        else
        {
            // - {KEY} : {VALUE}
            return new BlockSequenceMapping(Indent, IsRedirected, DataStyle.Normal,Writer).BeginMapping(tag, DataStyle.Normal);
        }
    }

    public override Sequence BeginSequence(string tag, DataStyle style)
    {
        if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return new FlowSequence(Indent, IsRedirected, DataStyle.Compact, Writer).BeginSequence(tag, DataStyle.Compact);
        }
        if (IsRedirected)
        {
            this.WriteScalar(tag);
        }
        return new BlockSequence(Math.Max(0, Indent) + 2, false, DataStyle.Normal, Writer);
    }

    public override Sequence Write<T>(Sequence context, T value, DataStyle style)
    {
        // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
        // "{NEWLINE}{INDENT}- {OUTPUT FROM WriteType}"
        // Special rule:
        // - The sequence identifier ("- ") does NOT use increased indentation.
        // - The indent can NOT be below 0
        // - Only the subsequent nodes follow deeper indentation levels.
        this.WriteScalar("\n");
        this.WriteScalar(new string(' ', Math.Max(Indent - 2, 0)));
        this.WriteScalar("- ");
        context.WriteType(value, style);
        return context;
    }
}
