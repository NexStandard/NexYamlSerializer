using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapKeySerializer : IEmitter
{
    public FlowMapKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State => EmitState.FlowMappingKey;

    public override EmitResult Begin(BeginContext context)
    {
        var current = context.Emitter.State;
        if (current is EmitState.BlockSequenceEntry)
        {
            WriteBlockSequenceEntryHeader();
        }
        else if (current is EmitState.FlowSequenceSecondaryEntry)
        {
            WriteFlowSequenceSeparator();
        }
        else if (current is EmitState.BlockMappingKey)
        {
            throw new InvalidOperationException($"To start flow-mapping in the {current} is not supported");
        }
        if (context.NeedsTag)
        {
            WriteRaw(context.Tag);
            WriteSpace();
        }
        WriteFlowMappingStart();
        return new EmitResult(this);
    }

    public override EmitResult WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        WriteMappingKeyFooter();
        return new EmitResult(machine.flowMapValueSerializer);
    }

    public override EmitResult End(IEmitter currentEmitter)
    {
        var needsLineBreak = false;
        switch (currentEmitter.State)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                break;
            case EmitState.BlockMappingValue:
                WriteFlowMappingEnd();
                WriteNewLine();
                return new EmitResult(machine.blockMapKeySerializer);
                break;
            case EmitState.FlowSequenceEntry:
                WriteFlowMappingEnd();
                return new EmitResult(machine.flowSequenceSecondarySerializer);
                break;
            case EmitState.FlowMappingValue:
                WriteFlowMappingEnd();
                return new EmitResult(machine.flowMapKeySerializer);
                break;
        }
        WriteFlowMappingEnd();

        if (needsLineBreak)
        {
            WriteNewLine();
        }
        return new EmitResult(currentEmitter);
    }
}
