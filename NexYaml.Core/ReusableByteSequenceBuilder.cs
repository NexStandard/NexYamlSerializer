using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace NexYaml.Core;

public static class ReusableByteSequenceBuilderPool
{
    private static readonly ConcurrentQueue<ReusableByteSequenceBuilder> queue = new();

    public static ReusableByteSequenceBuilder Rent()
    {
        if (queue.TryDequeue(out var builder))
            return builder;
        return new ReusableByteSequenceBuilder();
    }

    public static void Return(ReusableByteSequenceBuilder builder)
    {
        builder.Reset();
        queue.Enqueue(builder);
    }
}

internal class ReusableByteSequenceSegment : ReadOnlySequenceSegment<byte>
{
    private bool returnToPool;

    public ReusableByteSequenceSegment()
    {
        returnToPool = false;
    }

    public void SetBuffer(ReadOnlyMemory<byte> buffer, bool returnToPool)
    {
        Memory = buffer;
        this.returnToPool = returnToPool;
    }

    public void Reset()
    {
        if (returnToPool && MemoryMarshal.TryGetArray(Memory, out var segment) && segment.Array != null)
            ArrayPool<byte>.Shared.Return(segment.Array);
        Memory = default;
        RunningIndex = 0;
        Next = null;
    }

    public void SetRunningIndexAndNext(long runningIndex, ReusableByteSequenceSegment? nextSegment)
    {
        RunningIndex = runningIndex;
        Next = nextSegment;
    }
}

public class ReusableByteSequenceBuilder
{
    private readonly Stack<ReusableByteSequenceSegment> segmentPool = new();
    private readonly List<ReusableByteSequenceSegment> segments = [];

    public void Add(ReadOnlyMemory<byte> buffer, bool returnToPool)
    {
        if (!segmentPool.TryPop(out var segment))
            segment = new ReusableByteSequenceSegment();

        segment.SetBuffer(buffer, returnToPool);
        segments.Add(segment);
    }

    public bool TryGetSingleMemory(out ReadOnlyMemory<byte> memory)
    {
        if (segments.Count == 1)
        {
            memory = segments[0].Memory;
            return true;
        }
        memory = default;
        return false;
    }

    public ReadOnlySequence<byte> Build()
    {
        if (segments.Count == 0)
            return ReadOnlySequence<byte>.Empty;

        if (segments.Count == 1)
            return new ReadOnlySequence<byte>(segments[0].Memory);

        long running = 0;

        for (var i = 0; i < segments.Count; i++)
        {
            var next = i < segments.Count - 1 ? segments[i + 1] : null;
            segments[i].SetRunningIndexAndNext(running, next);
            running += segments[i].Memory.Length;
        }
        var firstSegment = segments[0];
        var lastSegment = segments[^1];
        return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
    }

    public void Reset()
    {
        foreach (var item in segments)
        {
            item.Reset();
            segmentPool.Push(item);
        }
        segments.Clear();
    }
}

