namespace NexYaml.Core;


/// <summary>
/// A manually managed, growable buffer used as a stack-like structure
/// for high-performance scenarios. Avoids allocations and GC pressure.
/// </summary>
/// <typeparam name="T">The element type to store in the buffer.</typeparam>
public sealed class ExpandBuffer<T>(int capacity) : IDisposable
{
    private const int MinimumGrow = 4;
    private const int GrowFactor = 200;
    private T[] buffer = new T[capacity];

    /// <summary>
    /// Provides direct reference access to the element at the specified index.
    /// </summary>
    public ref T this[int index] => ref buffer[index];

    /// <summary>
    /// Gets or sets the element at the top of the buffer.
    /// Throws if the buffer is empty.
    /// </summary>
    public T Current
    {
        get => this[Length - 1];
        set => this[Length - 1] = value;
    }

    /// <summary>
    /// Gets the element before the top element of the buffer.
    /// Throws if the buffer has fewer than two elements.
    /// </summary>
    public T Previous => this[Length - 2];

    /// <summary>
    /// Gets the number of elements currently in the buffer.
    /// </summary>
    public int Length { get; private set; } = 0;

    /// <summary>
    /// Marks the buffer as disposed. Invalidates future usage.
    /// </summary>
    public void Dispose()
    {
        if (Length < 0)
            return;
        Length = -1;
    }

    /// <summary>
    /// Returns a span over the currently used portion of the buffer.
    /// </summary>
    public Span<T> AsSpan()
    {
        return buffer.AsSpan(0, Length);
    }

    /// <summary>
    /// Returns a span over the specified number of elements.
    /// Ensures capacity before accessing.
    /// </summary>
    public Span<T> AsSpan(int length)
    {
        if (length > buffer.Length)
            SetCapacity(buffer.Length * 2);
        return buffer.AsSpan(0, length);
    }

    /// <summary>
    /// Resets the buffer without releasing the memory.
    /// </summary>
    public void Clear()
    {
        Length = 0;
    }

    /// <summary>
    /// Gets a reference to the element at the top of the buffer.
    /// </summary>
    public ref T Peek()
    {
        return ref buffer[Length - 1];
    }

    /// <summary>
    /// Removes and returns a reference to the top element.
    /// Throws if the buffer is empty.
    /// </summary>
    public ref T Pop()
    {
        if (Length == 0)
            throw new InvalidOperationException("Cannot pop the empty buffer");
        return ref buffer[--Length];
    }

    /// <summary>
    /// Attempts to pop the top element. Returns true on success.
    /// </summary>
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

    /// <summary>
    /// Adds a new element to the top of the buffer.
    /// Grows capacity if necessary.
    /// </summary>
    public void Add(T item)
    {
        if (Length >= buffer.Length)
            Grow();

        buffer[Length++] = item;
    }

    /// <summary>
    /// Sets a new capacity for the buffer, if larger than current.
    /// </summary>
    private void SetCapacity(int newCapacity)
    {
        if (buffer.Length >= newCapacity)
        {
            return;
        }

        var newBuffer = new T[newCapacity];
        Array.Copy(buffer, 0, newBuffer, 0, Length);
        buffer = newBuffer;
    }

    /// <summary>
    /// Grows the buffer based on the grow factor and minimum size constraints.
    /// </summary>
    private void Grow()
    {
        var newCapacity = buffer.Length * GrowFactor / 100;
        if (newCapacity < buffer.Length + MinimumGrow)
        {
            newCapacity = buffer.Length + MinimumGrow;
        }

        SetCapacity(newCapacity);
    }
}
