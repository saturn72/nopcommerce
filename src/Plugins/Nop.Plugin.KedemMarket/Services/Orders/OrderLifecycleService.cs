namespace KedemMarket.Services.Orders;

public class OrderLifecycleService : IOrderLifecycleService
{
    public Task CancelOrderAsync(string orderId, params string[] cancellationReasons)
    {
        throw new NotImplementedException("insert to user's orders sub-collection");
    }
}
