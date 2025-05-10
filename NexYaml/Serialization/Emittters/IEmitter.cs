using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;

/// <summary>
/// An abstract base class for YAML emitters that handle the writing of various YAML components
/// such as scalars, sequences, and mappings. This class defines the core logic for formatting
/// and emitting YAML content, including handling indentation, sequence entry headers, and flow sequences.
/// </summary>
public abstract class IEmitter(IYamlWriter writer, EmitterStateMachine machine)
{
    internal EmitterStateMachine StateMachine { get; } = machine;
    private static ReadOnlySpan<char> whitespaces => new char[]
    {
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '
    };

    /// <summary>
    /// The YAML writer used for emitting raw content.
    /// </summary>
    protected IYamlWriter writer = writer;

    /// <summary>
    /// The state machine that governs the emission state of the YAML document.
    /// </summary>
    protected EmitterStateMachine machine = machine;

    /// <summary>
    /// The current emission state of the emitter. This is used to determine the current mode of YAML writing.
    /// </summary>
    public abstract EmitState State { get; }

    /// <summary>
    /// Ends the current YAML structure being emitted (e.g., a sequence or a mapping).
    /// </summary>
    public abstract IEmitter End(IEmitter currentEmitter);

    /// <summary>
    /// Begins a new YAML structure.
    /// </summary>
    public abstract IEmitter Begin(BeginContext context);

    /// <summary>
    /// Writes a scalar value to the YAML document.
    /// </summary>
    /// <param name="value">The scalar value to be written.</param>
    public abstract IEmitter WriteScalar(ReadOnlySpan<char> value);

    /// <summary>
    /// Writes indentation to the YAML document. The indentation is determined by the current indentation level
    /// and can optionally be forced to a specific width.
    /// </summary>
    /// <param name="forceWidth">The width to force for indentation. If negative, the default width is used.</param>
    protected void WriteIndent(int forceWidth = -1)
    {
        int length;

        if (forceWidth > -1)
        {
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

    /// <summary>
    /// Writes a newline character to the YAML document.
    /// </summary>
    protected void WriteNewLine()
    {
        writer.WriteRaw(YamlCodes.NewLine);
    }

    /// <summary>
    /// Writes a sequence identifier (e.g., "- ") for a sequence entry.
    /// </summary>
    protected void WriteSequenceIdentifier()
    {
        writer.WriteRaw("- ");
    }

    /// <summary>
    /// Writes a flow sequence separator (e.g., ", ") for use in inline sequences.
    /// </summary>
    protected void WriteFlowSequenceSeparator()
    {
        writer.WriteRaw([',', ' ']);
    }

    /// <summary>
    /// Begins a flow sequence by writing the opening square bracket ("[").
    /// </summary>
    protected void WriteFlowSequenceStart()
    {
        writer.WriteRaw(['[']);
    }

    /// <summary>
    /// Ends a flow sequence by writing the closing square bracket ("]").
    /// </summary>
    protected void WriteFlowSequenceEnd()
    {
        writer.WriteRaw([']']);
    }

    /// <summary>
    /// Writes the footer for a mapping key (e.g., ": ") to separate keys from values in a mapping.
    /// </summary>
    protected void WriteMappingKeyFooter()
    {
        writer.WriteRaw(": ");
    }

    /// <summary>
    /// Writes an empty flow sequence (i.e., "[]") when no elements are present in the sequence.
    /// </summary>
    protected void WriteEmptyFlowSequence()
    {
        writer.WriteRaw(['[', ']']);
    }

    /// <summary>
    /// Writes an empty flow mapping (i.e., "{}") when no key-value pairs are present in the mapping.
    /// </summary>
    protected void WriteEmptyFlowMapping()
    {
        writer.WriteRaw(['{', '}']);
    }

    /// <summary>
    /// Begins a flow mapping by writing the opening curly brace ("{").
    /// </summary>
    protected void WriteFlowMappingStart()
    {
        writer.WriteRaw(['{', ' ']);
    }

    /// <summary>
    /// Ends a flow mapping by writing the closing curly brace ("}").
    /// </summary>
    protected void WriteFlowMappingEnd()
    {
        writer.WriteRaw([' ', '}']);
    }

    /// <summary>
    /// Writes a single space character to the YAML document.
    /// </summary>
    protected void WriteSpace()
    {
        writer.WriteRaw([' ']);
    }

    /// <summary>
    /// Writes a sequence separator (e.g., "- ") to separate sequence elements.
    /// </summary>
    protected void WriteSequenceSeparator()
    {
        writer.WriteRaw(['-', ' ']);
    }

    /// <summary>
    /// Writes raw characters to the YAML document without any formatting or modification.
    /// </summary>
    /// <param name="chars">The characters to write to the document.</param>
    protected void WriteRaw(ReadOnlySpan<char> chars)
    {
        writer.WriteRaw(chars);
    }

    /// <summary>
    /// Writes a single raw character to the YAML document.
    /// </summary>
    /// <param name="c">The character to write.</param>
    protected void WriteRaw(char c)
    {
        writer.WriteRaw(c);
    }
}
