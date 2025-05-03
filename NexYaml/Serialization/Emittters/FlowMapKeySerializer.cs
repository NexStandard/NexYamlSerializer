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

    public override void WriteScalar(ReadOnlySpan<char> value)
    {
        WriteRaw(value);
        WriteMappingKeyFooter();
        machine.Current = machine.Map(EmitState.FlowMappingValue);
    }

    public override void End()
    {
        machine.PopState();

        var needsLineBreak = false;
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                needsLineBreak = true;
                break;
            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                needsLineBreak = true;
                break;
            case EmitState.FlowSequenceEntry:
                machine.Current = machine.Map(EmitState.FlowSequenceSecondaryEntry);
                break;
            case EmitState.FlowMappingValue:
                machine.Current = machine.Map(EmitState.FlowMappingKey);
                break;
        }
        WriteFlowMappingEnd();

        if (needsLineBreak)
        {
            WriteNewLine();
        }
    }
}
