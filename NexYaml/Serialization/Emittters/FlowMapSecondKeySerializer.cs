using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapSecondKeySerializer : FlowMapKeySerializer
{
    public FlowMapSecondKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }
    public override void WriteScalar(ReadOnlySpan<char> value)
    {
        WriteFlowSequenceSeparator();
        WriteRaw(value);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.FlowMappingValue);
    }
}

