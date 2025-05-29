namespace ConcurrentCollectionsBenchmarks.Collections;

public sealed class BenchmarkingMpscQueue<T> : BenchmarkingProducerConsumerQueue<MpscQueue<T>, T>;