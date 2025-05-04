using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockSequenceEntrySerializer : IEmitter
{
    public BlockSequenceEntrySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State => EmitState.BlockSequenceEntry;

    public override IEmitter Begin(BeginContext context)
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
        return this;
    }

    public override IEmitter WriteScalar(ReadOnlySpan<char> output)
    {
        WriteIndent();
        WriteSequenceSeparator();
        WriteRaw(output);
        WriteNewLine();
        return this;
    }

    public override IEmitter End(IEmitter currentEmitter)
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
                return machine.blockMapKeySerializer;
        }
        return currentEmitter;
    }
}
