namespace Km.Orders.Services;

public interface IRateLimiter
{
    Task<bool> Limit(string key, int timeframeInMilisecs);
}
