namespace KedemMarket.Services.Orders;

public interface IOrderLifecycleService
{
    Task CancelOrderAsync(string orderId, params string[] cancellationReasons);
}
