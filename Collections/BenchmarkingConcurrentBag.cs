using System.Collections.Concurrent;

namespace ConcurrentCollectionsBenchmarks.Collections;

public sealed class BenchmarkingConcurrentBag<T> : BenchmarkingProducerConsumerQueue<ConcurrentBag<T>, T>;