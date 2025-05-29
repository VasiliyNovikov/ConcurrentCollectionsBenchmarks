using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ConcurrentCollectionsBenchmarks;

public interface IBenchmarkingQueue<T>
{
    void Enqueue(T item);
    T Dequeue();
}

public sealed class BenchmarkingQueue<T> : IBenchmarkingQueue<T>
{
    private readonly Queue<T> _queue = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _queue.Enqueue(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _queue.Dequeue();
}

public abstract class BenchmarkingProducerConsumerQueue<TQueue, T> : IBenchmarkingQueue<T>
    where TQueue : IProducerConsumerCollection<T>, new()
{
    private readonly TQueue _queue = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _queue.TryAdd(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _queue.TryTake(out var item) ? item : throw new InvalidOperationException("Queue is empty.");
}

public sealed class BenchmarkingConcurrentQueue<T> : BenchmarkingProducerConsumerQueue<ConcurrentQueue<T>, T>;

public sealed class BenchmarkingConcurrentBag<T> : BenchmarkingProducerConsumerQueue<ConcurrentBag<T>, T>;

public sealed class BenchmarkingMpscQueue<T> : BenchmarkingProducerConsumerQueue<MpscQueue<T>, T>;

public sealed class BenchmarkingBlockingCollection<T>(int? boundedCapacity = null) : IBenchmarkingQueue<T>
{
    private readonly BlockingCollection<T> _queue = boundedCapacity is null ? new() : new(boundedCapacity.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _queue.Add(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _queue.Take();
}

public sealed class BenchmarkingChannel<T> : IBenchmarkingQueue<T>
{
    private readonly ChannelReader<T> _reader;
    private readonly ChannelWriter<T> _writer;

    public BenchmarkingChannel(bool singleReader = false, bool singleWriter = false, int? boundedCapacity = null)
    {
        var channel = boundedCapacity is null
            ? Channel.CreateUnbounded<T>(new UnboundedChannelOptions { SingleReader = singleReader, SingleWriter = singleWriter })
            : Channel.CreateBounded<T>(new BoundedChannelOptions(boundedCapacity.Value) { SingleReader = singleReader, SingleWriter = singleWriter });
        _reader = channel.Reader;
        _writer = channel.Writer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item) => _writer.Write(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => _reader.Read();
}