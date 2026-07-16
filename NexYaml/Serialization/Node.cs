using Stride.Core;
using Stride.Graphics;
using Stride.Input;

namespace NexYaml.Serialization;

public struct Node
{
    public Node(int indent, bool isRedirected, DataStyle styleScope, Writer writer, NodeKind kind, bool skipFirstLineBreak= false)
    {
        // edge case for Blockmapping on a BlockSequence if there is no tag
        if(!isRedirected && skipFirstLineBreak)
        {
            this.isFirst = false;
        }
        Kind = kind;
        this.indent = indent;
        IsRedirected = isRedirected;
        StyleScope = styleScope;
        Writer = writer;
    }
    NodeKind Kind;
    private bool skipFirst = false;
    private bool isFirst = true;
    private int indent;
    public int Indent => indent;

    public bool IsRedirected { get; set; }
    public DataStyle StyleScope { get; init; }
    public Writer Writer { get; init; }

    public Node BeginMapping(string tag, DataStyle style)
    {
        if(Kind is NodeKind.Mapping)
        {

            if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
            {
                if (IsRedirected)
                {
                    WriteScalar(tag);
                    WriteScalar(" { ");
                }
                else
                {
                    WriteScalar("{ ");
                }
                // inside a flow, only new flows can be created, no block is allowed
                return new Node(Indent, false, DataStyle.Compact, Writer, NodeKind.Mapping, skipFirst);
            }
            if (IsRedirected)
            {
                WriteScalar(tag);
            }
            return new Node(Indent + 2, false, DataStyle.Normal, Writer, NodeKind.Mapping, skipFirst);
        }
        else if(Kind is NodeKind.Sequence)
        {
            if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
            {
                return new Node(Indent, IsRedirected, DataStyle.Compact, Writer, NodeKind.Mapping).BeginMapping(tag, DataStyle.Compact);
            }
            //  When no tag is provided, BlockMapping would introduce an extra faulty newline.
            if (IsRedirected)
            {
                // - {TAG}\n
                //   {KEY} : {VALUE}
                return new Node(Indent, IsRedirected, DataStyle.Compact, Writer, NodeKind.Mapping).BeginMapping(tag, DataStyle.Normal);
            }
            else
            {
                // - {KEY} : {VALUE}
                return new Node(Indent - 2, false, DataStyle.Normal, Writer, NodeKind.Mapping, true).BeginMapping(tag, DataStyle.Normal);
            }
        }
        throw new InvalidDataException();
    }

    public Node BeginSequence(string tag, DataStyle style)
    {
        if(Kind is NodeKind.Mapping)
        {

            if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
            {
                return new Node(Indent, IsRedirected, StyleScope, Writer,NodeKind.Sequence).BeginSequence(tag, DataStyle.Compact);
            }
            return new Node(Indent, IsRedirected, StyleScope, Writer, NodeKind.Sequence).BeginSequence(tag, DataStyle.Normal);
        }
        else if (Kind is NodeKind.Sequence)
        {
            if (StyleScope is DataStyle.Compact || style is DataStyle.Compact)
            {
                if (IsRedirected)
                {
                    WriteScalar(tag);
                    WriteScalar(" [ ");
                }
                else
                {
                    WriteScalar("[ ");
                }
                return new Node(Indent, false, DataStyle.Compact, Writer, NodeKind.Sequence);
            }
            else
            {
                if (IsRedirected)
                {
                    WriteScalar(tag);
                }
                return new Node(Math.Max(0, Indent) + 2, false, DataStyle.Normal, Writer, NodeKind.Sequence);
            }
        }
        throw new InvalidDataException();
    }
    public void WriteMap(Node context, ReadOnlySpan<char> key, DataStyle style)
    {
        if(StyleScope is DataStyle.Compact)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                WriteScalar(", ");
            }
            // First Node is {KEY: VALUE}
            int len = key.Length + 2;
            Span<char> buf = stackalloc char[len];
            key.CopyTo(buf);
            buf[key.Length] = ':';
            buf[key.Length + 1] = ' ';
            WriteScalar(buf);
        }
        else
        {
            if (skipFirst)
            {
                // "- {OUTPUT FROM WRITETYPE}"
                Span<char> x = stackalloc char[key.Length + 2];

                key.CopyTo(x.Slice(0, key.Length));
                x[^1] = ' ';
                x[^2] = ':';
                WriteScalar(x);
                skipFirst = false;
            }
            else
            {
                // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
                // "{NEWLINE}{INDENT}{KEY}: {OUTPUT FROM WriteType}"
                Span<char> x = stackalloc char[Indent + 1 + key.Length + 2];

                x[0] = '\n';
                x.Slice(1, Indent).Fill(' ');
                key.CopyTo(x.Slice(1 + Indent, key.Length));
                x[^1] = ' ';
                x[^2] = ':';
                WriteScalar(x);
            }
        }

    }
    public void WriteElement<T>(Node context, T value, DataStyle style)
    {
        if (StyleScope is DataStyle.Compact)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                WriteScalar(", ");
            }
            // First Node is {VALUE}
            this.WriteType(value, style);
        }
        else
        {
            // "{KEY}: {OPTIONAL TAG}" OR "- {OPTIONAL TAG}"
            // "{NEWLINE}{INDENT}- {OUTPUT FROM WriteType}"
            // Special rule:
            // - The sequence identifier ("- ") does NOT use increased indentation.
            // - The indent can NOT be below 0
            // - Only the subsequent nodes follow deeper indentation levels.
            int spaceCount = Math.Max(Indent - 2, 0);

            // total length: '\n' + spaces + "- "
            int len = 1 + spaceCount + 2;

            Span<char> buf = stackalloc char[len];
            buf.Fill(' ');
            int i = 0;
            buf[0] = '\n';
            buf[^2] = '-';

            WriteScalar(buf);
            context.WriteType(value, style);
        }
    }
    public void End()
    {
        if(Kind is NodeKind.Mapping && this.StyleScope is DataStyle.Compact)
        {
            WriteScalar(" }");
        }
        else if (Kind is NodeKind.Sequence && StyleScope is DataStyle.Compact)
        {
            WriteScalar(" ]");
        }
    }
    public void WriteScalar(ReadOnlySpan<char> text)
    {
        Writer.Write(text);
    }
    public void WriteScalar(ref ReadOnlySpan<char> text)
    {
        Writer.Write(text);
    }
    /// <summary>
    /// Writes an empty <see cref="Mapping"/> with the given tag.
    /// </summary>
    public void WriteEmptyMapping(string tag)
    {
        WriteScalar(tag);
        WriteScalar(" { }".AsSpan());
    }

    /// <summary>
    /// Writes an empty <see cref="Sequence"/> with the given tag.
    /// </summary>
    public void WriteEmptySequence(string tag)
    {
        WriteScalar(tag);
        WriteScalar(" [ ]".AsSpan());
    }
    /// <summary>
    /// Writes the provided text to the underlying output with formatting.
    /// </summary>
    public void WriteString(string value, DataStyle style = DataStyle.Compact)
    {
        WriteScalar(Writer.FormatString(this, value, style));
    }
}
