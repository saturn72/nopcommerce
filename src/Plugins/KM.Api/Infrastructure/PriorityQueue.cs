using Microsoft.Extensions.Caching.Memory;

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

        var callback = new PostEvictionCallbackRegistration
        {
            EvictionCallback = (object key, object? value, EvictionReason reason, object? state) =>
            {
                if (reason == EvictionReason.Removed || reason == EvictionReason.Replaced)
                    return;
                _ = handler((TData)value);
            }
        };

        var mco = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
        };
        mco.PostEvictionCallbacks.Add(callback);
     
        _memoryCache.Set(key, data, mco);
    }
}
