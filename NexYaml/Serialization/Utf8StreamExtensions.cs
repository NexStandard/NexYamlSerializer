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
    public static EmitState PreviousState(this IUTF8Stream stream)
    {
        return stream.Previous.State;
    }
    public static IEmitter Map(this IUTF8Stream stream, EmitState state)
    {
        return stream.EmitterFactory.Map(state);
    }

    public static void WriteRaw(this IUTF8Stream stream,string value)
    {
        stream.WriteRaw(StringEncoding.Utf8.GetBytes(value));
    }
    public static bool IsFirstElement(this IUTF8Stream stream)
    {
        return stream.ElementCount == 0;
    }

    public static void WriteBlockSequenceEntryHeader(this IUTF8Stream stream)
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

    public static void WriteRaw(this IUTF8Stream stream, ReadOnlySpan<char> value)
    {
        var length = StringEncoding.Utf8.GetByteCount(value);
        byte[]? rented = null;
        Span<byte> buf = length <= 256
          ? stackalloc byte[1024]
          : rented = ArrayPool<byte>.Shared.Rent(length);
        
        var bytesWritten = StringEncoding.Utf8.GetBytes(value, buf);

        stream.WriteRaw(buf[..length]);

        if (rented is not null)
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public static void WriteScalar(this IUTF8Stream stream, string value)
    {
        stream.WriteScalar(StringEncoding.Utf8.GetBytes(value));
    }
    public static Span<char> TryRemoveDuplicateLineBreak(this IUTF8Stream stream,Span<char> scalarChars)
    {
        if (stream.Current.State is EmitState.BlockMappingValue or EmitState.BlockSequenceEntry)
        {
            scalarChars = scalarChars[..^1];
        }
        return scalarChars;
    }
    // https://gist.github.com/IXLLEGACYIXL/f06b792b482bf796a44c7a9b5ebab890
    public static IUTF8Stream WriteIndent(this IUTF8Stream stream, int forceWidth = -1)
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
    public static void WriteScalar(this IUTF8Stream stream, ReadOnlySpan<char> value)
    {

        var length = StringEncoding.Utf8.GetByteCount(value);
        byte[]? rented = null;
        Span<byte> buf = length <= 256
          ? stackalloc byte[1024]
          : rented = ArrayPool<byte>.Shared.Rent(length);

        var bytesWritten = StringEncoding.Utf8.GetBytes(value, buf);

        stream.WriteScalar(buf[..length]);

        if (rented is not null)
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
    public static IUTF8Stream WriteRaw(this IUTF8Stream stream, ReadOnlySpan<byte> value, bool lineBreak)
    {
        stream.WriteRaw(value);
        if (lineBreak)
        {
            stream.WriteNewLine();
        }
        return stream;
    }
    public static IUTF8Stream WriteFlowSequenceSeparator(this IUTF8Stream stream)
    {
        stream.WriteRaw([',', ' ']);
        return stream;
    }
    public static IUTF8Stream WriteFlowSequenceStart(this IUTF8Stream stream)
    {
        stream.WriteRaw(['[']);
        return stream;
    }
    public static IUTF8Stream WriteFlowSequenceEnd(this IUTF8Stream stream)
    {
        stream.WriteRaw([']']);
        return stream;
    }
    public static IUTF8Stream WriteMappingKeyFooter(this IUTF8Stream stream)
    {
        stream.WriteRaw(": ");
        return stream;
    }
    public static IUTF8Stream WriteEmptyFlowSequence(this IUTF8Stream stream)
    {
        stream.WriteRaw(['[', ']']);
        return stream;
    }
    public static IUTF8Stream WriteEmptyFlowMapping(this IUTF8Stream stream)
    {
        stream.WriteRaw(['{', '}']);
        return stream;
    }
    public static IUTF8Stream WriteFlowMappingStart(this IUTF8Stream stream)
    {
        stream.WriteRaw(['{', ' ']);
        return stream;
    }
    public static IUTF8Stream WriteFlowMappingEnd(this IUTF8Stream stream)
    {
        stream.WriteRaw([' ', '}']);
        return stream;
    }
    public static IUTF8Stream WriteSpace(this IUTF8Stream stream)
    {
        stream.WriteRaw([' ']);
        return stream;
    }
    public static IUTF8Stream WriteSequenceSeparator(this IUTF8Stream stream)
    {
        stream.WriteRaw(['-', ' ']);
        return stream;
    }
    public static IUTF8Stream WriteNewLine(this IUTF8Stream stream)
    {
        stream.WriteRaw((byte)'\n');
        return stream;
    }
}
