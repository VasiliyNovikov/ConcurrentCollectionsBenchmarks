using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ConcurrentCollectionsBenchmarks.Collections;

public abstract class BenchmarkingProducerConsumerQueue<TQueue, T> : IBenchmarkingQueue<T>
    where TQueue : IProducerConsumerCollection<T>, new()
{
    private readonly TQueue _queue = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _queue.TryAdd(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _queue.TryTake(out var item) ? item : throw new InvalidOperationException("Queue is empty.");
}