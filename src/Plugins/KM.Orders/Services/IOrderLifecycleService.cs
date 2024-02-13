
namespace Km.Orders.Services
{
    public interface IOrderLifecycleService
    {
        Task CancelOrderAsync(string orderId, params string[] cancellationReasons);
    }
}
