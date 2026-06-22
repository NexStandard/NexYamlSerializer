using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowMapping : Mapping
{
    public FlowMapping(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
        : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping BeginMapping(string tag, DataStyle style)
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
        return new FlowMapping(Indent, false, DataStyle.Compact, Writer);
    }

    public override Sequence BeginSequence(string tag, DataStyle style)
    {
        // inside a flow, only new flows can be created, no block is allowed
        return new FlowSequence(Indent, IsRedirected, StyleScope, Writer)
            .BeginSequence(tag, DataStyle.Compact);
    }
    public override void End()
    {
        WriteScalar(" }");
    }

    public override Mapping WriteKey(Mapping context, string key, DataStyle style)
    {
        // First Node is {KEY: VALUE}
        int len = key.Length + 2; 
        Span<char> buf = stackalloc char[len];
        key.CopyTo(buf);
        buf[key.Length] = ':';
        buf[key.Length + 1] = ' ';
        WriteScalar(buf);
        return new FlowMappingSecondary(Indent,false,StyleScope,Writer);
    }
}
