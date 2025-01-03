using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using System.Buffers;

namespace NexYaml.Serialization;
public static class Utf8StreamExtensions
{
    private static ReadOnlySpan<char> whitespaces => new char[]
        {
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '
        };
    public static EmitState PreviousState(this UTF8Stream stream)
    {
        return stream.Previous.State;
    }
    public static IEmitter Map(this UTF8Stream stream, EmitState state)
    {
        return stream.EmitterFactory.Map(state);
    }

    public static void WriteRaw(this UTF8Stream stream,string value)
    {
        stream.WriteRaw(StringEncoding.Utf8.GetBytes(value));
    }
    public static bool IsFirstElement(this UTF8Stream stream)
    {
        return stream.ElementCount == 0;
    }

    public static void WriteBlockSequenceEntryHeader(this UTF8Stream stream)
    {
        if (stream.IsFirstElement())
        {
            switch (stream.Previous.State)
            {
                case EmitState.BlockSequenceEntry:
                    stream.WriteRaw(YamlCodes.Lf);
                    stream.IndentationManager.IncreaseIndent();
                    break;
                case EmitState.BlockMappingValue:
                    stream.WriteRaw(YamlCodes.Lf);
                    break;
            }
        }
        stream.WriteIndent();
        stream.WriteRaw(stream.settings.SequenceIdentifier);
    }

    public static void WriteRaw(this UTF8Stream stream, ReadOnlySpan<char> value)
    {
        var length = StringEncoding.Utf8.GetByteCount(value);
        byte[]? rented = null;
        Span<byte> buf = length <= 256
          ? stackalloc byte[256]
          : rented = ArrayPool<byte>.Shared.Rent(length);
        
        var bytesWritten = StringEncoding.Utf8.GetBytes(value, buf);

        stream.WriteRaw(buf[..length]);

        if (rented is not null)
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public static void WriteScalar(this UTF8Stream stream, string value)
    {
        stream.WriteScalar(StringEncoding.Utf8.GetBytes(value));
    }
    public static Span<char> TryRemoveDuplicateLineBreak(this UTF8Stream stream,Span<char> scalarChars)
    {
        if (stream.Current.State is EmitState.BlockMappingValue or EmitState.BlockSequenceEntry)
        {
            scalarChars = scalarChars[..^1];
        }
        return scalarChars;
    }

    public static UTF8Stream WriteIndent(this UTF8Stream stream, int forceWidth = -1)
    {
        int length;

        if (forceWidth > -1)
        {
            if (forceWidth <= 0)
                return stream;
            length = forceWidth;
        }
        else if (stream.CurrentIndentLevel > 0)
        {
            length = stream.CurrentIndentLevel * stream.IndentWidth;
        }
        else
        {
            return stream;
        }
        var whiteSpaces = Utf8StreamExtensions.whitespaces;

        int toWrite = length;
        while (toWrite > 0)
        {
            var slice = whiteSpaces.Slice(0, Math.Min(whiteSpaces.Length, toWrite));
            stream.WriteRaw(slice);
            toWrite -= slice.Length;
        }
        return stream;
    }
    public static void WriteScalar(this UTF8Stream stream, ReadOnlySpan<char> value)
    {

        var length = StringEncoding.Utf8.GetByteCount(value);
        byte[]? rented = null;
        Span<byte> buf = length <= 256
          ? stackalloc byte[256]
          : rented = ArrayPool<byte>.Shared.Rent(length);

        var bytesWritten = StringEncoding.Utf8.GetBytes(value, buf);

        stream.WriteScalar(buf[..length]);

        if (rented is not null)
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
    public static UTF8Stream WriteRaw(this UTF8Stream stream, ReadOnlySpan<byte> value, bool lineBreak)
    {
        stream.WriteRaw(value);
        if (lineBreak)
        {
            stream.WriteNewLine();
        }
        return stream;
    }
    public static UTF8Stream WriteFlowSequenceSeparator(this UTF8Stream stream)
    {
        stream.WriteRaw([',', ' ']);
        return stream;
    }
    public static UTF8Stream WriteFlowSequenceStart(this UTF8Stream stream)
    {
        stream.WriteRaw(['[']);
        return stream;
    }
    public static UTF8Stream WriteFlowSequenceEnd(this UTF8Stream stream)
    {
        stream.WriteRaw([']']);
        return stream;
    }
    public static UTF8Stream WriteMappingKeyFooter(this UTF8Stream stream)
    {
        stream.WriteRaw(": ");
        return stream;
    }
    public static UTF8Stream WriteEmptyFlowSequence(this UTF8Stream stream)
    {
        stream.WriteRaw(['[', ']']);
        return stream;
    }
    public static UTF8Stream WriteEmptyFlowMapping(this UTF8Stream stream)
    {
        stream.WriteRaw(['{', '}']);
        return stream;
    }
    public static UTF8Stream WriteFlowMappingStart(this UTF8Stream stream)
    {
        stream.WriteRaw(['{', ' ']);
        return stream;
    }
    public static UTF8Stream WriteFlowMappingEnd(this UTF8Stream stream)
    {
        stream.WriteRaw([' ', '}']);
        return stream;
    }
    public static UTF8Stream WriteSpace(this UTF8Stream stream)
    {
        stream.WriteRaw([' ']);
        return stream;
    }
    public static UTF8Stream WriteSequenceSeparator(this UTF8Stream stream)
    {
        stream.WriteRaw(['-', ' ']);
        return stream;
    }
    public static UTF8Stream WriteNewLine(this UTF8Stream stream)
    {
        stream.WriteRaw((byte)'\n');
        return stream;
    }
}
