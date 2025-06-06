﻿using Stride.Core;

namespace NexYaml.Serialization.Nodes;

class FlowSequenceSecondary : FlowSequence
{
    public override WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
    {
        if (context.IsRedirected)
        {
            context.WriteScalar(tag);
            context.WriteScalar(" [ ");
        }
        else
        {
            context.WriteScalar("[ ");
        }
        return new WriteContext<Sequence>(context.Indent, false, DataStyle.Compact, CommonNodes.FlowSequence, context.Writer);
    }
    public override WriteContext<Sequence> Write<T>(WriteContext<Sequence> context, T value, DataStyle style)
    {
        // Node following a FlowMapping is prefixed with comma ", {VALUE}"
        context.WriteScalar(", ");
        return base.Write(context, value, style);
    }
}
