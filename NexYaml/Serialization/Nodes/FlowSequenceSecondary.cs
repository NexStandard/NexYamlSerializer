using Silk.NET.OpenXR;
using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequenceSecondary : FlowSequence
{
    public FlowSequenceSecondary(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
    : base(indent, isRedirected, styleScope, writer)
    {
    }
    public override Sequence BeginSequence(string tag, DataStyle style)
    {
        if (IsRedirected)
        {
            this.WriteScalar(tag);
            this.WriteScalar(" [ ");
        }
        else
        {
            this.WriteScalar("[ ");
        }
        return new FlowSequence(Indent, false, DataStyle.Compact, Writer);
    }
    public override Sequence Write<T>(Sequence context, T value, DataStyle style)
    {
        // Node following a FlowMapping is prefixed with comma ", {VALUE}"
        this.WriteScalar(", ");
        return base.Write(context, value, style);
    }
}
