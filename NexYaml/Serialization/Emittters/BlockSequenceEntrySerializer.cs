using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockSequenceEntrySerializer : IEmitter
{
    public BlockSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State => EmitState.BlockSequenceEntry;

    public override EmitResult Begin(BeginContext context)
    {
        if (context.NeedsTag)
        {
            WriteRaw(context.Tag);
            WriteNewLine();
        }
        switch (context.Emitter.State)
        {
            case EmitState.BlockSequenceEntry:
                WriteBlockSequenceEntryHeader();
                context.Indentation.IncreaseIndent();
                WriteNewLine();
                break;
            case EmitState.BlockMappingValue:
                WriteNewLine();
                break;
        }
        return new EmitResult(this);
    }

    public override EmitResult WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteSequenceSeparator();
        WriteRaw(output);
        WriteNewLine();
        return new EmitResult(this);
    }

    public override EmitResult End(IEmitter currentEmitter)
    {
        switch (currentEmitter.State)
        {
            case EmitState.BlockSequenceEntry:
                machine.IndentationManager.DecreaseIndent();
                break;

            case EmitState.BlockMappingKey:
                throw new YamlException("Complex key is not supported.");

            case EmitState.BlockMappingValue:
                machine.IndentationManager.DecreaseIndent();
                return new EmitResult(machine.blockMapKeySerializer);
        }
        return new EmitResult(currentEmitter);
    }
}
