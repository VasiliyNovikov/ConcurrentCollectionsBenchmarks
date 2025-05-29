using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ConcurrentCollectionsBenchmarks.Collections;

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