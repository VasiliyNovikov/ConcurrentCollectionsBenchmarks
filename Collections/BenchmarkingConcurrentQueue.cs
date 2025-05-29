using System.Collections.Concurrent;

namespace ConcurrentCollectionsBenchmarks.Collections;

public sealed class BenchmarkingConcurrentQueue<T> : BenchmarkingProducerConsumerQueue<ConcurrentQueue<T>, T>;