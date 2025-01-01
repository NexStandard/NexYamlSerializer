namespace NexYaml.Core;

public class InsertionQueue<T>
{
    private const int MinimumGrow = 4;
    private const int GrowFactor = 200;
    private T[] array;
    private int headIndex;
    private int tailIndex;

    public InsertionQueue(int capacity)
    {
        var cap = capacity < 0 ? 0 : capacity;
        array = new T[cap];
        headIndex = tailIndex = Count = 0;
    }

    public int Count
    {
        get;
        private set;
    }

    public T Peek()
    {
        if (Count == 0)
            ThrowForEmptyQueue();
        return array[headIndex];
    }

    public void Enqueue(T item)
    {
        if (Count == array.Length)
            Grow();

        array[tailIndex] = item;
        MoveNext(ref tailIndex);
        Count++;
    }

    public T Dequeue()
    {
        if (Count == 0)
            ThrowForEmptyQueue();

        var removed = array[headIndex];
        MoveNext(ref headIndex);
        Count--;
        return removed;
    }

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

    private void Grow()
    {
        var newCapacity = (int)((long)array.Length * GrowFactor / 100);
        if (newCapacity < array.Length + MinimumGrow)
            newCapacity = array.Length + MinimumGrow;
        SetCapacity(newCapacity);
    }

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

    private void MoveNext(ref int index)
    {
        index = (index + 1) % array.Length;
    }

    private static void ThrowForEmptyQueue()
    {
        throw new InvalidOperationException("EmptyQueue");
    }
}

