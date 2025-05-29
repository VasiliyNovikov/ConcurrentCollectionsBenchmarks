namespace ConcurrentCollectionsBenchmarks;

public interface IBenchmarkingQueue<T>
{
    void Enqueue(T item);
    T Dequeue();
}