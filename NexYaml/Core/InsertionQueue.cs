namespace NexYaml.Core;

/// <summary>
/// A circular buffer-based FIFO queue that supports insertion at arbitrary positions
/// relative to the queue head. Automatically grows when capacity is reached.
/// </summary>
/// <typeparam name="T">The type of elements stored in the queue.</typeparam>
internal class InsertionQueue<T>
{
    private const int MinimumGrow = 4;
    private const int GrowFactor = 200;
    private T[] array;
    private int headIndex;
    private int tailIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertionQueue{T}"/> class with a specified capacity.
    /// </summary>
    /// <param name="capacity">Initial capacity of the internal buffer. If less than 0, defaults to 0.</param>
    public InsertionQueue(int capacity)
    {
        var cap = capacity < 0 ? 0 : capacity;
        array = new T[cap];
        headIndex = tailIndex = Count = 0;
    }

    /// <summary>
    /// Gets the number of elements currently stored in the queue.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Returns the element at the front of the queue without removing it.
    /// </summary>
    /// <returns>The element at the head of the queue.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
    public T Peek()
    {
        if (Count == 0)
            ThrowForEmptyQueue();
        return array[headIndex];
    }

    /// <summary>
    /// Adds an element to the end of the queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void Enqueue(T item)
    {
        if (Count == array.Length)
            Grow();

        array[tailIndex] = item;
        MoveNext(ref tailIndex);
        Count++;
    }

    /// <summary>
    /// Removes and returns the element at the front of the queue.
    /// </summary>
    /// <returns>The removed element.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
    public T Dequeue()
    {
        if (Count == 0)
            ThrowForEmptyQueue();

        var removed = array[headIndex];
        MoveNext(ref headIndex);
        Count--;
        return removed;
    }

    /// <summary>
    /// Inserts an item at a specified logical position relative to the head.
    /// </summary>
    /// <param name="posTo">Position to insert the item at. 0 = head, Count = tail.</param>
    /// <param name="item">Item to insert.</param>
    /// <remarks>No bounds check is performed. Caller must ensure posTo ∈ [0, Count].</remarks>
    public void Insert(int posTo, T item)
    {
        if (Count == array.Length)
            Grow();

        MoveNext(ref tailIndex);
        Count++;

        for (var pos = Count - 1; pos > posTo; pos--)
        {
            var index = (headIndex + pos) % array.Length;
            var indexPrev = index == 0 ? array.Length - 1 : index - 1;
            array[index] = array[indexPrev];
        }
        array[(posTo + headIndex) % array.Length] = item;
    }

    /// <summary>
    /// Grows the internal buffer to accommodate more elements.
    /// Growth is based on a percentage factor and minimum increment.
    /// </summary>
    private void Grow()
    {
        var newCapacity = (int)((long)array.Length * GrowFactor / 100);
        if (newCapacity < array.Length + MinimumGrow)
        {
            newCapacity = array.Length + MinimumGrow;
        }

        SetCapacity(newCapacity);
    }

    /// <summary>
    /// Sets the internal capacity of the buffer and reorders existing elements.
    /// </summary>
    /// <param name="capacity">The new capacity.</param>
    private void SetCapacity(int capacity)
    {
        var newArray = new T[capacity];
        if (Count > 0)
        {
            if (headIndex < tailIndex)
            {
                Array.Copy(array, headIndex, newArray, 0, Count);
            }
            else
            {
                Array.Copy(array, headIndex, newArray, 0, array.Length - headIndex);
                Array.Copy(array, 0, newArray, array.Length - headIndex, tailIndex);
            }
        }

        array = newArray;
        headIndex = 0;
        tailIndex = Count == capacity ? 0 : Count;
    }

    /// <summary>
    /// Moves the specified index forward by one position in circular fashion.
    /// </summary>
    /// <param name="index">The index to advance.</param>
    private void MoveNext(ref int index)
    {
        index = (index + 1) % array.Length;
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> for empty queue operations.
    /// </summary>
    private static void ThrowForEmptyQueue()
    {
        throw new InvalidOperationException("EmptyQueue");
    }
}
