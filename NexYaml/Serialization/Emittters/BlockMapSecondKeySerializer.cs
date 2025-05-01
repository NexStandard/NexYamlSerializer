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
    public override void End()
    {
        machine.PopState();
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;
            case EmitState.BlockMappingValue:
                machine.IndentationManager.DecreaseIndent();
                machine.Current = machine.Map(EmitState.BlockMappingSecondaryKey);
                break;
            case EmitState.FlowMappingValue:
                // TODO: This case is not implemented, further clarification needed
                throw new NotImplementedException();
        }
    }
}
