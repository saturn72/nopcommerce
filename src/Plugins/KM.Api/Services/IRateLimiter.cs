namespace Km.Api.Services;

public interface IRateLimiter
{
    Task<bool> Limit(string key, int timeframeInMilisecs);
}
