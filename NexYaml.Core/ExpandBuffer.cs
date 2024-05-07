using System.Buffers;

namespace NexYaml.Core;

public sealed class ExpandBuffer<T>(int capacity) : IDisposable
{
    const int MinimumGrow = 4;
    const int GrowFactor = 200;

    T[] buffer = ArrayPool<T>.Shared.Rent(capacity);

    public ref T this[int index]
    {
        get => ref buffer[index];
    }
    /// <summary>
    /// Gets the current highest element on the Stack.
    /// Set replaces the current highest element with the value
    /// </summary>
    public T Current
    {
        get => this[^1];
        set => this[^1] = value;
    }
    public T Previous
    {
        get => this[^2];
    }
    public int Length { get; private set; } = 0;

    public void Dispose()
    {
        if (Length < 0) 
            return;
        ArrayPool<T>.Shared.Return(buffer);
        Length = -1;
    }

    public Span<T> AsSpan() => buffer.AsSpan(0, Length);

    public Span<T> AsSpan(int length)
    {
        if (length > buffer.Length)
            SetCapacity(buffer.Length * 2);
        return buffer.AsSpan(0, length);
    }

    public void Clear()
    {
        Length = 0;
    }

    public ref T Peek() => ref buffer[Length - 1];

    public ref T Pop()
    {
        if (Length == 0)
            throw new InvalidOperationException("Cannot pop the empty buffer");
        return ref buffer[--Length];
    }

    public bool TryPop(out T value)
    {
        if (Length == 0)
        {
            value = default!;
            return false;
        }
        value = Pop();
        return true;
    }

    public void Add(T item)
    {
        if (Length >= buffer.Length)
            Grow();

        buffer[Length++] = item;
    }

    void SetCapacity(int newCapacity)
    {
        if (buffer.Length >= newCapacity) 
            return;

        var newBuffer = ArrayPool<T>.Shared.Rent(newCapacity);
        Array.Copy(buffer, 0, newBuffer, 0, Length);
        ArrayPool<T>.Shared.Return(buffer);
        buffer = newBuffer;
    }

    void Grow()
    {
        var newCapacity = buffer.Length * GrowFactor / 100;
        if (newCapacity < buffer.Length + MinimumGrow)
            newCapacity = buffer.Length + MinimumGrow;
        SetCapacity(newCapacity);
    }
}

