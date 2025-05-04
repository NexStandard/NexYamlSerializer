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
    public override EmitResult WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteRaw(output);
        WriteMappingKeyFooter();
        return new EmitResult(machine.blockMapValueSerializer);
    }
    public override EmitResult End(IEmitter currentEmitter)
    {
        switch (currentEmitter.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;
            case EmitState.BlockMappingValue:
                machine.IndentationManager.DecreaseIndent();
                return new EmitResult(machine.blockMapSecondaryKeySerializer);
            case EmitState.FlowMappingValue:
                // TODO: This case is not implemented, further clarification needed
                throw new NotImplementedException();
        }
        return new EmitResult(currentEmitter);
    }
}
