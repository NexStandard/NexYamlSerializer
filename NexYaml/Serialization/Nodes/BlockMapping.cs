using NexYaml.Core.Serialization.Nodes;
using Stride.Core;
using Stride.Input;

namespace NexYaml.Serialization.Nodes;

public class BlockMapping : Mapping
{
    public BlockMapping(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
    : base(indent, isRedirected, styleScope, writer)
    {
    }

    private bool isFirst = true;
    public override Mapping BeginMapping(string tag, DataStyle style)
    {
        if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
        {
            if (IsRedirected)
            {
                WriteScalar(tag);
                WriteScalar(" { ");
            }
            else
            {
                WriteScalar("{ ");
            }
            // inside a flow, only new flows can be created, no block is allowed
            return new BlockMapping(Indent, false, DataStyle.Compact, Writer);
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
            return new BlockSequence(Indent, IsRedirected, StyleScope, Writer).BeginSequence(tag, DataStyle.Compact);
        }
        return new BlockSequence(Indent, IsRedirected, StyleScope, Writer).BeginSequence(tag, DataStyle.Normal);
    }
    public override Mapping WriteKey(Mapping context, ReadOnlySpan<char> key, DataStyle style)
    {
        if(StyleScope is DataStyle.Compact)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                WriteScalar(", ");
            }
            // First Node is {KEY: VALUE}
            int len = key.Length + 2;
            Span<char> buf = stackalloc char[len];
            key.CopyTo(buf);
            buf[key.Length] = ':';
            buf[key.Length + 1] = ' ';
            WriteScalar(buf);
            return this;
        }
        else
        {
            // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
            // "{NEWLINE}{INDENT}{KEY}: {OUTPUT FROM WriteType}"
            Span<char> x = stackalloc char[Indent + 1 + key.Length + 2];

            x[0] = '\n';
            x.Slice(1, Indent).Fill(' ');
            key.CopyTo(x.Slice(1 + Indent, key.Length));
            x[^1] = ' ';
            x[^2] = ':';
            WriteScalar(x);
            return this;
        }

    }
    public override void End()
    {
        if(StyleScope is DataStyle.Compact)
        {
            WriteScalar(" }");
        }
    }
}
