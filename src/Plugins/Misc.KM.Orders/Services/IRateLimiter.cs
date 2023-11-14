namespace Nop.Plugin.Misc.KM.Orders.Services
{
    public interface IRateLimiter
    {
        Task<bool> Limit(string key, int timeframeInMilisecs);
    }
}
