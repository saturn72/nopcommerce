
namespace KM.Api.Services.Checkout;

public interface IKmOrderService
{
    Task<IEnumerable<CreateOrderResponse>> CreateOrdersAsync(IEnumerable<CreateOrderRequest> requests);
}
