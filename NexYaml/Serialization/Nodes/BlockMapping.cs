using Stride.Core;
using Stride.Input;

namespace NexYaml.Serialization.Nodes;

class BlockMapping : Mapping
{
    public BlockMapping(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
    : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping BeginMapping(string tag, DataStyle style)
    {
        if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return new FlowMapping(Indent, IsRedirected, StyleScope, Writer).BeginMapping(tag,DataStyle.Compact);
        }
        if (IsRedirected)
        {
            WriteScalar(tag);
        }
        return new BlockMapping(Indent + 2, false, DataStyle.Normal, Writer);
    }

    public override Sequence BeginSequence(string tag, DataStyle style)
    {
        if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            return new FlowSequence(Indent, IsRedirected, StyleScope, Writer).BeginSequence(tag, DataStyle.Compact);
        }
        return new BlockSequence(Indent, IsRedirected, StyleScope, Writer).BeginSequence(tag, DataStyle.Normal);
    }
    public override Mapping WriteKey(Mapping context, string key, DataStyle style)
    {
        // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
        // "{NEWLINE}{INDENT}{KEY}: {OUTPUT FROM WriteType}"
        Span<char> x = stackalloc char[Indent + 1 + key.Length+2];

        x[0] = '\n';
        x.Slice(1, Indent).Fill(' ');
        key.AsSpan().CopyTo(x.Slice(1 + Indent,key.Length));
        x[^1] = ' ';
        x[^2] = ':';
        WriteScalar(x);
        return this;
    }
}
