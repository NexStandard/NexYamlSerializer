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
    public override IEmitter WriteScalar(ReadOnlySpan<char> value)
    {
        WriteFlowSequenceSeparator();
        WriteRaw(value);
        WriteMappingKeyFooter();
        return machine.flowMapValueSerializer;
    }
}