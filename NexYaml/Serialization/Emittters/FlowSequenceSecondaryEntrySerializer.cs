using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.Emittters;
internal class FlowSequenceSecondaryEntrySerializer : FlowSequenceEntrySerializer
{
    public FlowSequenceSecondaryEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }
    public override EmitState State => EmitState.FlowSequenceSecondaryEntry;
    public override EmitResult WriteScalar(ReadOnlySpan<char> value)
    {
        WriteFlowSequenceSeparator();
        WriteRaw(value);
        return new EmitResult(machine.flowSequenceSecondarySerializer);
    }
}
