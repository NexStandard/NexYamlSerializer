using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockSequenceEntrySerializer : IEmitter
{
    public BlockSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.BlockSequenceEntry;

    public override void Begin()
    { 
        if (machine.TryGetTag(out var tag))
        {
            WriteRaw(tag);
            WriteNewLine();
        }
        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteBlockSequenceEntryHeader();
                machine.IndentationManager.IncreaseIndent();
                WriteNewLine();
                break;
            case EmitState.BlockMappingValue:
                WriteNewLine();
                break;
        }
        machine.Next = machine.Map(State);
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteSequenceSeparator();
        WriteRaw(output);
        WriteNewLine();
    }

    public override void End()
    {
        machine.PopState();

        switch (machine.Current.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;

            case EmitState.BlockMappingKey:
                throw new YamlException("Complex key is not supported.");

            case EmitState.BlockMappingValue:
                machine.Current = machine.Map(EmitState.BlockMappingKey);
                break;
        }
    }
}
