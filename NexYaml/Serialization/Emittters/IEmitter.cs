using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
public abstract class IEmitter(YamlWriter writer, EmitterStateMachine machine)
{
    private static ReadOnlySpan<char> whitespaces => new char[]
    {
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '
    };
    protected YamlWriter writer = writer;
    protected EmitterStateMachine machine = machine;
    public abstract EmitState State { get; }
    public abstract void End();
    public abstract void Begin();
    public abstract void WriteScalar(ReadOnlySpan<char> value);
    protected void WriteBlockSequenceEntryHeader()
    {
        if (machine.IsFirstElement)
        {
            switch (machine.Previous.State)
            {
                case EmitState.BlockSequenceEntry:
                    writer.WriteRaw(YamlCodes.NewLine);
                    machine.IndentationManager.IncreaseIndent();
                    break;
                case EmitState.BlockMappingValue:
                    writer.WriteRaw(YamlCodes.NewLine);
                    break;
            }
        }
        WriteIndent();
        WriteSequenceIdentifier();
    }
    protected void WriteIndent(int forceWidth = -1)
    {
        int length;

        if (forceWidth > -1)
        {
            if (forceWidth <= 0)
                return;
            length = forceWidth;
        }
        else if (machine.CurrentIndentLevel > 0)
        {
            length = machine.CurrentIndentLevel * machine.IndentationManager.IndentWidth;
        }
        else
        {
            return;
        }
        var whiteSpaces = whitespaces;

        int toWrite = length;
        while (toWrite > 0)
        {
            var slice = Math.Min(whiteSpaces.Length, toWrite);
            writer.WriteRaw(whiteSpaces[..slice]);
            toWrite -= slice;
        }
    }
    protected void WriteNewLine()
    {
        writer.WriteRaw(YamlCodes.NewLine);
    }
    protected void WriteSequenceIdentifier()
    {
        writer.WriteRaw("- ");
    }
    protected void WriteFlowSequenceSeparator()
    {
        writer.WriteRaw([',', ' ']);
    }
    protected void WriteFlowSequenceStart()
    {
        writer.WriteRaw(['[']);
    }
    protected void WriteFlowSequenceEnd()
    {
        writer.WriteRaw([']']);
    }
    protected void WriteMappingKeyFooter()
    {
        writer.WriteRaw(": ");
    }
    protected void WriteEmptyFlowSequence()
    {
        writer.WriteRaw(['[', ']']);
    }
    protected void WriteEmptyFlowMapping()
    {
        writer.WriteRaw(['{', '}']);
    }
    protected void WriteFlowMappingStart()
    {
        writer.WriteRaw(['{', ' ']);
    }
    protected void WriteFlowMappingEnd()
    {
        writer.WriteRaw([' ', '}']);
    }
    protected void WriteSpace()
    {
        writer.WriteRaw([' ']);
    }
    protected void WriteSequenceSeparator()
    {
        writer.WriteRaw(['-', ' ']);
    }
    protected void WriteRaw(ReadOnlySpan<char> chars)
    {
        writer.WriteRaw(chars);
    }
    protected void WriteRaw(char c)
    {
        writer.WriteRaw(c);
    }
}