using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;
using NexYaml.Core;

namespace NexYaml.Parser;

internal readonly struct ScalarPool(int capacity) : IDisposable
{
    private readonly ExpandBuffer<Scalar> queue = new(capacity);

    public readonly Scalar Rent()
    {
        return queue.TryPop(out var scalar)
            ? scalar
            : new Scalar();
    }

    public readonly void Return(Scalar scalar)
    {
        scalar.Clear();
        queue.Add(scalar);
    }

    public readonly void Dispose()
    {
        queue.Dispose();
        for (var i = 0; i < queue.Length; i++)
        {
            queue[i].Dispose();
        }
    }
}
public sealed record Scalar : ITokenContent, IDisposable
{
    private const int GrowFactor = 200;
    private const int MinimumGrow = 4;

    private char[] buffer;
    public int Length { get; private set; }

    public static readonly Scalar Null = new(0);

    public Scalar(int capacity = MinimumGrow)
    {
        buffer = ArrayPool<char>.Shared.Rent(capacity);
    }

    public Scalar(ReadOnlySpan<char> content)
    {
        buffer = ArrayPool<char>.Shared.Rent(content.Length);
        Write(content);
    }

    public ReadOnlySpan<char> AsSpan()
    {
        return buffer.AsSpan(0, Length);
    }

    public ReadOnlySpan<char> AsSpan(int start, int length)
    {
        return buffer.AsSpan(start, length);
    }

    public void Write(char code)
    {
        EnsureCapacity(Length + 1);
        buffer[Length++] = code;
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
        EnsureCapacity(Length + codes.Length);
        codes.CopyTo(buffer.AsSpan(Length));
        Length += codes.Length;
    }

    public void WriteUnicodeCodepoint(int codepoint)
    {
        Span<char> chars = [(char)codepoint];
        var utf8ByteCount = Encoding.UTF8.GetByteCount(chars);
        Span<byte> utf8Bytes = stackalloc byte[utf8ByteCount];
        // TODO
        // Write(utf8Bytes);
    }

    public void Clear()
    {
        Length = 0;
    }

    public void Dispose()
    {
        if (buffer != null)
        {
            ArrayPool<char>.Shared.Return(buffer);
            buffer = null!;
        }
        Length = -1; // Mark as disposed
    }

    public override string ToString()
    {
        return AsSpan().ToString();
    }

    public bool IsNull()
    {
        return buffer.Length == 0 || buffer.SequenceEqual(YamlCodes.Null);
    }

    public bool SequenceEqual(ReadOnlySpan<char> span)
    {
        return AsSpan().SequenceEqual(span);
    }

    private void EnsureCapacity(int sizeHint)
    {
        if (sizeHint <= buffer.Length)
            return;

        var newCapacity = buffer.Length * GrowFactor / 100;
        if (newCapacity < sizeHint)
            newCapacity = sizeHint;

        SetCapacity(newCapacity);
    }

    private void SetCapacity(int newCapacity)
    {
        var newBuffer = ArrayPool<char>.Shared.Rent(newCapacity);
        buffer.AsSpan(0, Length).CopyTo(newBuffer);
        ArrayPool<char>.Shared.Return(buffer);
        buffer = newBuffer;
    }
}
