using System.Runtime.CompilerServices;

namespace ConcurrentCollectionsBenchmarks.Collections;

public sealed class BenchmarkingQueue<T> : IBenchmarkingQueue<T>
{
    private readonly Queue<T> _queue = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _queue.Enqueue(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _queue.Dequeue();
}