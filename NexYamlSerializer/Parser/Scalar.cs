#nullable enable
using NexYaml.Core;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime.CompilerServices;

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

class Scalar : ITokenContent, IDisposable
{
    const int MinimumGrow = 4;
    const int GrowFactor = 200;

    public static readonly Scalar Null = new(0);

    public int Length { get; private set; }
    byte[] buffer;

    public Scalar(int capacity)
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
        if (Length == buffer.Length)
        {
            Grow();
        }

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
        Grow(Length + codes.Length);
        codes.CopyTo(buffer.AsSpan(Length, codes.Length));
        Length += codes.Length;
    }

    public void WriteUnicodeCodepoint(int codepoint)
    {
        Span<char> chars = [(char)codepoint];
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
        if (Length < 0) 
            return;
        ArrayPool<byte>.Shared.Return(buffer);
        Length = -1;
    }

    public override string ToString()
    {
        return StringEncoding.Utf8.GetString(AsSpan());
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// !!null | !!Null | !!NULL | ~
    /// </remarks>
    public bool IsNull()
    {
        var span = AsSpan();
        switch (span.Length)
        {
            case 0:
            case 4 when span.SequenceEqual(YamlCodes.Null0) ||
                        span.SequenceEqual(YamlCodes.Null1) ||
                        span.SequenceEqual(YamlCodes.Null2):
                return true;
            default:
                return false;
        }
    }

    public bool SequenceEqual(ReadOnlySpan<byte> span)
    {
        return AsSpan().SequenceEqual(span);
    }

    public void Grow(int sizeHint)
    {
        if (sizeHint <= buffer.Length)
        {
            return;
        }
        var newCapacity = buffer.Length * GrowFactor / 100;
        while (newCapacity < sizeHint)
        {
            newCapacity = newCapacity * GrowFactor / 100;
        }
        SetCapacity(newCapacity);
    }

    void Grow()
    {
        var newCapacity = buffer.Length * GrowFactor / 100;
        if (newCapacity < buffer.Length + MinimumGrow)
        {
            newCapacity = buffer.Length + MinimumGrow;
        }
        SetCapacity(newCapacity);
    }

    void SetCapacity(int newCapacity)
    {
        if (buffer.Length >= newCapacity) 
            return;

        var newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
        Array.Copy(buffer, 0, newBuffer, 0, Length);
        ArrayPool<byte>.Shared.Return(buffer);
        buffer = newBuffer;
    }
}

