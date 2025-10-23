using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;
using NexYaml.Core;

namespace NexYaml.Parser;

public sealed class Scalar : ITokenContent
{
    private string content = "";
    public int Length => content.Length;

    public Scalar()
    {
    }

    public Scalar(ReadOnlySpan<char> content)
    {
        this.content = content.ToString();
    }

    public ReadOnlySpan<char> AsSpan()
    {
        return content.AsSpan();
    }

    public ReadOnlySpan<char> AsSpan(int start, int length)
    {
        return content.AsSpan(start,length);
    }

    public void Write(char code)
    {
        content += code;
    }

    public void Write(LineBreakState lineBreak)
    {
        switch (lineBreak)
        {
            case LineBreakState.None:
                break;
            case LineBreakState.Lf:
                Write(YamlCodes.Lf);
                break;
            case LineBreakState.CrLf:
                Write(YamlCodes.Cr);
                Write(YamlCodes.Lf);
                break;
            case LineBreakState.Cr:
                Write(YamlCodes.Cr);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lineBreak), lineBreak, null);
        }
    }

    public void Write(ReadOnlySpan<char> codes)
    {
        content += codes.ToString();
    }

    public void WriteUnicodeCodepoint(int codepoint)
    {
        Span<char> chars = [(char)codepoint];
        content += chars.ToString();
    }

    public void Clear()
    {
        content = string.Empty;
    }

    public override string ToString()
    {
        return AsSpan().ToString();
    }

    public bool IsNull()
    {
        return content.Length == 0 || content.AsSpan().SequenceEqual(YamlCodes.Null);
    }

    public bool SequenceEqual(ReadOnlySpan<char> span)
    {
        return content.AsSpan().SequenceEqual(span);
    }
}
