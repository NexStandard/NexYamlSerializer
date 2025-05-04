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
    public override IEmitter WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteRaw(output);
        WriteMappingKeyFooter();
        return machine.blockMapValueSerializer;
    }
    public override IEmitter End(IEmitter currentEmitter)
    {
        switch (currentEmitter.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;
            case EmitState.BlockMappingValue:
                machine.IndentationManager.DecreaseIndent();
                return machine.blockMapSecondaryKeySerializer;
            case EmitState.FlowMappingValue:
                // TODO: This case is not implemented, further clarification needed
                throw new NotImplementedException();
        }
        return currentEmitter;
    }
}
