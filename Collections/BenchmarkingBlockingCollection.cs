using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ConcurrentCollectionsBenchmarks.Collections;

public sealed class BenchmarkingBlockingCollection<T>(int? boundedCapacity = null) : IBenchmarkingQueue<T>
{
    private readonly BlockingCollection<T> _queue = boundedCapacity is null ? new() : new(boundedCapacity.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _queue.Add(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _queue.Take();
}