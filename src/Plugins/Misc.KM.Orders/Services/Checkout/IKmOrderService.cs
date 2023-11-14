
namespace Nop.Plugin.Misc.KM.Orders.Services.Checkout;

public interface IKmOrderService
{
    Task<IEnumerable<CreateOrderResponse>> CreateOrdersAsync(IEnumerable<CreateOrderRequest> requests);
}
