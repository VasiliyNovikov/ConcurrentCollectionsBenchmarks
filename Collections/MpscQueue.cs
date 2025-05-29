using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ConcurrentCollectionsBenchmarks.Collections;

public sealed class MpscQueue<T> : IProducerConsumerCollection<T>
{
    private Node? _pool;
    private volatile Node _head;
    private Node _tail;

    public MpscQueue() => _head = _tail = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        var node = Rent(item);
        var prev = Interlocked.Exchange(ref _head, node);
        prev.Next = node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue([MaybeNullWhen(false)] out T item)
    {
        var next = _tail.Next;
        if (next is null)
        {
            item = default;
            return false;
        }

        item = next.Value;
        var old = _tail;
        _tail = next;
        Return(old);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Node Rent(T value)
    {
        while (true)
        {
            var node = Volatile.Read(ref _pool);
            if (node is null)
                return new(value);

            var next = node.Next;
            if (Interlocked.CompareExchange(ref _pool, next, node) != node)
                continue;

            node.Value = value;
            node.Next = null;
            return node;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Return(Node node)
    {
        node.Value = default!;
        node.Next = _pool;
        Volatile.Write(ref _pool, node);
    }

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private sealed class Node(T value = default!)
    {
        public T Value = value;
        public Node? Next;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

    void ICollection.CopyTo(Array array, int index) => throw new NotSupportedException();

    int ICollection.Count => throw new NotSupportedException();

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => throw new NotSupportedException();

    void IProducerConsumerCollection<T>.CopyTo(T[] array, int index) => throw new NotSupportedException();

    T[] IProducerConsumerCollection<T>.ToArray() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IProducerConsumerCollection<T>.TryAdd(T item)
    {
        Enqueue(item);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IProducerConsumerCollection<T>.TryTake([MaybeNullWhen(false)] out T item) => TryDequeue(out item);
}