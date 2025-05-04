using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowSequenceEntrySerializer : IEmitter
{
    public FlowSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.FlowSequenceEntry;

    public override IEmitter Begin(BeginContext context)
    {
        if (context.NeedsTag)
        {
            WriteRaw(context.Tag);
            WriteSpace();
        }
        switch (context.Emitter.State)
        {
            case EmitState.BlockMappingKey or EmitState.FlowMappingKey or EmitState.FlowMappingSecondaryKey or EmitState.BlockMappingSecondaryKey:
                throw new YamlException("To start flow-sequence in the mapping key is not supported.");
            default:
                WriteFlowSequenceStart();
            break;
        }
        return this;
    }

    public override IEmitter WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        return machine.flowSequenceSecondarySerializer;
    }

    public override IEmitter End(IEmitter currentEmitter)
    {
        WriteFlowSequenceEnd();
        switch (currentEmitter.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteNewLine();
                break;
            case EmitState.BlockMappingValue:
                WriteNewLine();
                return machine.blockMapKeySerializer;
            case EmitState.FlowMappingValue:
                return machine.flowMapSecondarySerializer;
            case EmitState.FlowSequenceEntry:
                break;
        }
        return currentEmitter;
    }
}
