using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class FlowMapKeySerializer : IEmitter
{
    public FlowMapKeySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State => EmitState.FlowMappingKey;

    public override IEmitter Begin(BeginContext context)
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
        return this;
    }

    public override IEmitter WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        WriteMappingKeyFooter();
        return machine.flowMapValueSerializer;
    }

    public override IEmitter End(IEmitter currentEmitter)
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
                return machine.blockMapKeySerializer;
                break;
            case EmitState.FlowSequenceEntry:
                WriteFlowMappingEnd();
                return machine.flowSequenceSecondarySerializer;
                break;
            case EmitState.FlowMappingValue:
                WriteFlowMappingEnd();
                return machine.flowMapKeySerializer;
                break;
        }
        WriteFlowMappingEnd();

        if (needsLineBreak)
        {
            WriteNewLine();
        }
        return currentEmitter;
    }
}
