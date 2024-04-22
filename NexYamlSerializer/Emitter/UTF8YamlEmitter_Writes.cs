using NexYaml.Core;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NexVYaml.Emitter;
internal partial class Utf8YamlEmitter
{
    static byte[] whiteSpaces =
    [
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
            (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ', (byte)' ',
    ];

    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        var offset = 0;
        var output = Writer.GetSpan(CalculateMaxScalarBufferLength(value.Length));

        BeginScalar(output, ref offset);
        value.CopyTo(output[offset..]);
        offset += value.Length;
        EndScalar(output, ref offset);
    }

    internal void WriteIndent(Span<byte> output, ref int offset, int forceWidth = -1)
    {
        int length;
        if (forceWidth > -1)
        {
            if (forceWidth <= 0) 
                return;
            length = forceWidth;
        }
        else if (CurrentIndentLevel > 0)
        {
            length = CurrentIndentLevel * Options.IndentWidth;
        }
        else
        {
            return;
        }

        if (length > whiteSpaces.Length)
        {
            whiteSpaces = Enumerable.Repeat(YamlCodes.Space, length * 2).ToArray();
        }
        whiteSpaces.AsSpan(0, length).CopyTo(output[offset..]);
        offset += length;
    }

    internal void WriteRaw(byte value)
    {
        var output = Writer.GetSpan(1);
        output[0] = value;
        Writer.Advance(1);
    }

    internal void WriteRaw(ReadOnlySpan<byte> value, bool indent, bool lineBreak)
    {
        var length = value.Length +
                     (indent ? CurrentIndentLevel * Options.IndentWidth : 0) +
                     (lineBreak ? 1 : 0);

        var offset = 0;
        var output = Writer.GetSpan(length);
        if (indent)
        {
            WriteIndent(output, ref offset);
        }
        value.CopyTo(output[offset..]);
        if (lineBreak)
        {
            output[length - 1] = YamlCodes.Lf;
        }
        Writer.Advance(length);
    }

    internal void WriteRaw(ReadOnlySpan<byte> value1, ReadOnlySpan<byte> value2, bool indent, bool lineBreak)
    {
        var length = value1.Length + value2.Length +
                     (indent ? CurrentIndentLevel * Options.IndentWidth : 0) +
                     (lineBreak ? 1 : 0);
        var offset = 0;
        var output = Writer.GetSpan(length);
        if (indent)
        {
            WriteIndent(output, ref offset);
        }

        value1.CopyTo(output[offset..]);
        offset += value1.Length;

        value2.CopyTo(output[offset..]);
        if (lineBreak)
        {
            output[length - 1] = YamlCodes.Lf;
        }
        Writer.Advance(length);
    }

    internal void WriteBlockSequenceEntryHeader()
    {
        if (IsFirstElement)
        {
            switch (StateStack.Previous)
            {
                case EmitState.BlockSequenceEntry:
                    WriteRaw(YamlCodes.Lf);
                    IndentationManager.IncreaseIndent();
                    break;
                case EmitState.BlockMappingValue:
                    WriteRaw(YamlCodes.Lf);
                    break;
            }
        }
        WriteRaw(EmitCodes.BlockSequenceEntryHeader, true, false);
    }

    public int CalculateMaxScalarBufferLength(int length)
    {
        var around = ((CurrentIndentLevel + 1) * Options.IndentWidth) + 3;
        if (tagStack.Length > 0)
        {
            length += StringEncoding.Utf8.GetMaxByteCount(tagStack.Peek().Length) + around; // TODO:
        }
        return length;
    }

    internal void PushState(EmitState state)
    {
        StateStack.Add(state);
        elementCountStack.Add(currentElementCount);
        currentElementCount = 0;
    }

    internal void PopState()
    {
        StateStack.Pop();
        currentElementCount = elementCountStack.Length > 0 ? elementCountStack.Pop() : 0;
    }
    public void Tag(ref string value)
    {
        tagStack.Add(value);
    }
}
