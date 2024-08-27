
namespace KM.Api.Services.Checkout;

public interface IKmOrderService
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
}
