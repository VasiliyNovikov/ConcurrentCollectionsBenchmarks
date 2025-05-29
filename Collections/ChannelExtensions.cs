using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ConcurrentCollectionsBenchmarks.Collections;

public static class ChannelExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this ChannelWriter<T> channel, T item, int spinCount = 10, int spinWaitCount = 10)
    {
        for (var i = 0; i < spinCount; ++i)
        {
            if (channel.TryWrite(item))
                return;
        }

        for (var i = 0; i < spinWaitCount; ++i)
        {
            if (channel.TryWrite(item))
                return;
            Thread.SpinWait(1 << i);
        }

        channel.WriteAsync(item).AsTask().Wait();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(this ChannelReader<T> channel, int spinCount = 10, int spinWaitCount = 10)
    {
        for (var i = 0; i < spinCount; ++i)
        {
            if (channel.TryRead(out var item))
                return item;
        }

        for (var i = 0; i < spinWaitCount; ++i)
        {
            if (channel.TryRead(out var item))
                return item;
            Thread.SpinWait(1 << i);
        }

        return channel.ReadAsync().AsTask().Result;
    }
}
