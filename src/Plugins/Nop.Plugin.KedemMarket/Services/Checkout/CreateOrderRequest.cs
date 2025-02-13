using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;

namespace KedemMarket.Services.Checkout;
public record CreateOrderRequest
{
    public IEnumerable<ShoppingCartItem> CartItems { get; init; }
    public string PaymentMethod { get; init; }
    public bool StorePickup { get; init; }
    public Address BillingInfo { get; init; }
    public bool UpdateBillingInfo { get; init; }
    public Address ShippingInfo { get; init; }
    public bool UpdateShippingInfo { get; init; }
}
