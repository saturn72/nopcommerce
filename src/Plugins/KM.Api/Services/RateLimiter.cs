using Microsoft.Extensions.Caching.Memory;

namespace KM.Api.Services
{
    public class RateLimiter : IRateLimiter
    {
        private readonly IMemoryCache _cache;

        public RateLimiter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> Limit(string key, int timeframeInMilisecs)
        {
            var shouldLimit = false;
            _ = await _cache.GetOrCreateAsync(key, ce =>
            {
                shouldLimit = true;
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(timeframeInMilisecs);
                ce.Value = key;
                return Task.FromResult(key);
            });

            return shouldLimit;
        }
    }
}
