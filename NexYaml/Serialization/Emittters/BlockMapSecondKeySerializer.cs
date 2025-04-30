using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapSecondKeySerializer : BlockMapKeySerializer
{
    public BlockMapSecondKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }
    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteRaw(output);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.BlockMappingValue);
    }
}
