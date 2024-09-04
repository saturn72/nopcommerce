using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;

namespace KM.Api.Infrastructure;

public class PriorityQueue : IPriorityQueue
{
    private readonly IMemoryCache _memoryCache;
    public PriorityQueue(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Dequeue(string key) => _memoryCache.Remove(key);
    public void Enqueue<TData>(string key, TData data, Func<TData, Task> handler, DateTimeOffset absoluteExpiration = default)
    {
        key.ThrowIfNullOrEmpty("\'key\' parameter cannot be null or empty");
        handler.ThrowArgumentNullException("\'handler\' parameter cannot be null or empty");

        if (absoluteExpiration == default)
            absoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(10);


        var mceo = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromSeconds(3),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20),
        };

        mceo.RegisterPostEvictionCallback((object key, object? value, EvictionReason reason, object? state) =>
            {
                if (reason == EvictionReason.Removed || reason == EvictionReason.Replaced)
                    return;
                _ = handler((TData)value);
            });

        _memoryCache.Set(key, data, mceo);
    }
}
