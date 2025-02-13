namespace KedemMarket.Services.Checkout;

public interface IKmOrderService
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
}
