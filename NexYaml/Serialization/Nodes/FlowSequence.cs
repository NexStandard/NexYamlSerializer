using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequence : Sequence
{
    public FlowSequence(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
    : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Mapping BeginMapping(string tag, DataStyle style)
    {
        // inside a flow, only new flows can be created, no block is allowed
        return new FlowMapping(Indent, IsRedirected, StyleScope, Writer).BeginMapping(tag,style);
    }
    private bool isFirst = true;
    public override Sequence BeginSequence(string tag, DataStyle style)
    {
        if (IsRedirected)
        {
            WriteScalar(tag);
            WriteScalar(" [ ");
        }
        else
        {
            WriteScalar("[ ");
        }
        // inside a flow, only new flows can be created, no block is allowed
        return new FlowSequence(Indent, false, DataStyle.Compact, Writer);
    }

    public override Sequence Write<T>(Sequence context, T value, DataStyle style)
    {
        if (isFirst)
        {
            isFirst = false;
        }
        else
        {
            WriteScalar(", ");
        }
        // First Node is {VALUE}
        this.WriteType(value, style);

        // all following Nodes need a prefix
        return this;
    }
    public override void End()
    {
        WriteScalar(" ]");
    }
}
