#nullable enable
using NexYaml.Core;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace NexVYaml.Parser;

readonly struct ScalarPool(int capacity) : IDisposable
{
    readonly ExpandBuffer<Scalar> queue = new(capacity);

    public readonly Scalar Rent()
    {
        return queue.TryPop(out var scalar)
            ? scalar
            : new Scalar(32);
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
public class Scalar : ITokenContent, IDisposable
{
    private const int GrowFactor = 200;
    private const int MinimumGrow = 4;

    private byte[] buffer;
    public int Length { get; private set; }

    public static readonly Scalar Null = new(0);

    public Scalar(int capacity = MinimumGrow)
    {
        buffer = ArrayPool<byte>.Shared.Rent(capacity);
    }

    public Scalar(ReadOnlySpan<byte> content)
    {
        buffer = ArrayPool<byte>.Shared.Rent(content.Length);
        Write(content);
    }

    public Span<byte> AsSpan() => buffer.AsSpan(0, Length);

    public Span<byte> AsSpan(int start, int length) => buffer.AsSpan(start, length);

    public void Write(byte code)
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

    public void Write(ReadOnlySpan<byte> codes)
    {
        EnsureCapacity(Length + codes.Length);
        codes.CopyTo(buffer.AsSpan(Length));
        Length += codes.Length;
    }

    public void WriteUnicodeCodepoint(int codepoint)
    {
        Span<char> chars = stackalloc char[1];
        chars[0] = (char)codepoint;
        var utf8ByteCount = StringEncoding.Utf8.GetByteCount(chars);
        Span<byte> utf8Bytes = stackalloc byte[utf8ByteCount];
        StringEncoding.Utf8.GetBytes(chars, utf8Bytes);
        Write(utf8Bytes);
    }

    public void Clear()
    {
        Length = 0;
    }

    public void Dispose()
    {
        if (buffer != null)
        {
            ArrayPool<byte>.Shared.Return(buffer);
            buffer = null!;
        }
        Length = -1; // Mark as disposed
    }

    public override string ToString()
    {
        return StringEncoding.Utf8.GetString(AsSpan());
    }

    public bool IsNull()
    {
        var span = AsSpan();
        return span.Length == 0 || span.SequenceEqual(YamlCodes.Null0);
    }

    public bool SequenceEqual(ReadOnlySpan<byte> span)
    {
        return AsSpan().SequenceEqual(span);
    }

    private void EnsureCapacity(int sizeHint)
    {
        if (sizeHint <= buffer.Length)
            return;

        int newCapacity = buffer.Length * GrowFactor / 100;
        if (newCapacity < sizeHint)
            newCapacity = sizeHint;

        SetCapacity(newCapacity);
    }

    private void SetCapacity(int newCapacity)
    {
        var newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
        buffer.AsSpan(0, Length).CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(buffer);
        buffer = newBuffer;
    }
}