using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using ConcurrentCollectionsBenchmarks.Collections;

namespace ConcurrentCollectionsBenchmarks;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[SimpleJob(RunStrategy.Throughput, warmupCount:10, iterationCount:100, invocationCount:1)]
public class Benchmarks
{
    private const int TestItemCount = 10000;
    private const int InitialCapacity = TestItemCount * 2;
    private static readonly object DummyObject = new();
    private static readonly object[] InitialItems = Enumerable.Repeat(DummyObject, InitialCapacity).ToArray();

    private readonly BenchmarkingQueue<object> _queue = new();
    private readonly BenchmarkingConcurrentQueue<object> _concurrentQueue = new();
    private readonly BenchmarkingConcurrentBag<object> _concurrentBag = new();
    private readonly BenchmarkingMpscQueue<object> _mpscQueue = new();
    private readonly BenchmarkingBlockingCollection<object> _blockingQueue = new();
    private readonly BenchmarkingBlockingCollection<object> _boundedBlockingQueue = new(TestItemCount * 4);
    private readonly BenchmarkingChannel<object> _channel = new(singleReader: true, singleWriter: false);

    [GlobalSetup]
    public void Setup()
    {
        foreach (var item in InitialItems)
        {
            _queue.Enqueue(item);
            _concurrentQueue.Enqueue(item);
            _concurrentBag.Enqueue(item);
            _mpscQueue.Enqueue(item);
            _blockingQueue.Enqueue(item);
            _boundedBlockingQueue.Enqueue(item);
            _channel.Enqueue(item);
        }
    }

    #region Enqueue
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Enqueue<T>(T queue) where T : IBenchmarkingQueue<object>
    {
        for (var i = 0; i < TestItemCount; ++i)
            queue.Enqueue(DummyObject);
    }

    private static void Enqueue_Cleanup<T>(T queue) where T : IBenchmarkingQueue<object>
    {
        for (var i = 0; i < TestItemCount; ++i)
            queue.Dequeue();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Enqueue")]
    public void Queue_Enqueue() => Enqueue(_queue);

    [IterationCleanup(Target = nameof(Queue_Enqueue))]
    public void Queue_Enqueue_Cleanup() => Enqueue_Cleanup(_queue);

    [Benchmark]
    [BenchmarkCategory("Enqueue")]
    public void ConcurrentQueue_Enqueue() => Enqueue(_concurrentQueue);

    [IterationCleanup(Target = nameof(ConcurrentQueue_Enqueue))]
    public void ConcurrentQueue_Enqueue_Cleanup() => Enqueue_Cleanup(_concurrentQueue);

    [Benchmark]
    [BenchmarkCategory("Enqueue")]
    public void ConcurrentBag_Enqueue() => Enqueue(_concurrentBag);

    [IterationCleanup(Target = nameof(ConcurrentBag_Enqueue))]
    public void ConcurrentBag_Enqueue_Cleanup() => Enqueue_Cleanup(_concurrentBag);

    [Benchmark]
    [BenchmarkCategory("Enqueue")]
    public void MpscQueue_Enqueue() => Enqueue(_mpscQueue);

    [IterationCleanup(Target = nameof(MpscQueue_Enqueue))]
    public void MpscQueue_Enqueue_Cleanup() => Enqueue_Cleanup(_mpscQueue);

    [Benchmark]
    [BenchmarkCategory("Enqueue")]
    public void BlockingCollection_Enqueue() => Enqueue(_blockingQueue);

    [IterationCleanup(Target = nameof(BlockingCollection_Enqueue))]
    public void BlockingCollection_Enqueue_Cleanup() => Enqueue_Cleanup(_blockingQueue);

    [Benchmark]
    [BenchmarkCategory("Enqueue")]
    public void BoundedBlockingCollection_Enqueue() => Enqueue(_boundedBlockingQueue);

    [IterationCleanup(Target = nameof(BoundedBlockingCollection_Enqueue))]
    public void BoundedBlockingCollection_Enqueue_Cleanup() => Enqueue_Cleanup(_boundedBlockingQueue);

    [Benchmark]
    [BenchmarkCategory("Enqueue")]
    public void Channel_Enqueue() => Enqueue(_channel);

    [IterationCleanup(Target = nameof(Channel_Enqueue))]
    public void Channel_Enqueue_Cleanup() => Enqueue_Cleanup(_channel);

    #endregion

    #region Dequeue

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object? Dequeue<T>(T queue) where T : IBenchmarkingQueue<object>
    {
        object? item = null;
        for (var i = 0; i < TestItemCount; ++i)
            item = queue.Dequeue();
        return item;
    }

    private static void Dequeue_Cleanup<T>(T queue) where T : IBenchmarkingQueue<object>
    {
        for (var i = 0; i < TestItemCount; ++i)
            queue.Enqueue(DummyObject);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Dequeue")]
    public object? Queue_Dequeue() => Dequeue(_queue);

    [IterationCleanup(Target = nameof(Queue_Dequeue))]
    public void Queue_Dequeue_Cleanup() => Dequeue_Cleanup(_queue);
    
    [Benchmark]
    [BenchmarkCategory("Dequeue")]
    public object? ConcurrentQueue_Dequeue() => Dequeue(_concurrentQueue);

    [IterationCleanup(Target = nameof(ConcurrentQueue_Dequeue))]
    public void ConcurrentQueue_Dequeue_Cleanup() => Dequeue_Cleanup(_concurrentQueue);
    
    [Benchmark]
    [BenchmarkCategory("Dequeue")]
    public object? ConcurrentBag_Dequeue() => Dequeue(_concurrentBag);
    
    [IterationCleanup(Target = nameof(ConcurrentBag_Dequeue))]
    public void ConcurrentBag_Dequeue_Cleanup() => Dequeue_Cleanup(_concurrentBag);

    [Benchmark]
    [BenchmarkCategory("Dequeue")]
    public object? MpscQueue_Dequeue() => Dequeue(_mpscQueue);

    [IterationCleanup(Target = nameof(MpscQueue_Dequeue))]
    public void MpscQueue_Dequeue_Cleanup() => Dequeue_Cleanup(_mpscQueue);

    [Benchmark]
    [BenchmarkCategory("Dequeue")]
    public object? BlockingCollection_Dequeue() => Dequeue(_blockingQueue);
    
    [IterationCleanup(Target = nameof(BlockingCollection_Dequeue))]
    public void BlockingCollection_Dequeue_Cleanup() => Dequeue_Cleanup(_blockingQueue);

    [Benchmark]
    [BenchmarkCategory("Dequeue")]
    public object? BoundedBlockingCollection_Dequeue() => Dequeue(_boundedBlockingQueue);

    [IterationCleanup(Target = nameof(BoundedBlockingCollection_Dequeue))]
    public void BoundedBlockingCollection_Dequeue_Cleanup() => Dequeue_Cleanup(_boundedBlockingQueue);

    [Benchmark]
    [BenchmarkCategory("Dequeue")]
    public object? Channel_Dequeue() => Dequeue(_channel);

    [IterationCleanup(Target = nameof(Channel_Dequeue))]
    public void Channel_Dequeue_Cleanup() => Dequeue_Cleanup(_channel);

    #endregion

    #region Enqueue / Dequeue
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object? EnqueueDequeue<T>(T queue) where T : IBenchmarkingQueue<object>
    {
        object? item = null;
        for (var i = 0; i < TestItemCount; ++i)
        {
            queue.Enqueue(DummyObject);
            item = queue.Dequeue();
        }
        return item;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public object? Queue_EnqueueDequeue() => EnqueueDequeue(_queue);
    
    [Benchmark]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public object? ConcurrentQueue_EnqueueDequeue() => EnqueueDequeue(_concurrentQueue);
    
    [Benchmark]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public object? ConcurrentBag_EnqueueDequeue() => EnqueueDequeue(_concurrentBag);

    [Benchmark]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public object? MpscQueue_EnqueueDequeue() => EnqueueDequeue(_mpscQueue);

    [Benchmark]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public object? BlockingCollection_EnqueueDequeue() => EnqueueDequeue(_blockingQueue);

    [Benchmark]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public void BoundedBlockingCollection_EnqueueDequeue() => EnqueueDequeue(_boundedBlockingQueue);
    
    [Benchmark]
    [BenchmarkCategory("Enqueue / Dequeue")]
    public void Channel_EnqueueDequeue() => EnqueueDequeue(_channel);

    #endregion
}