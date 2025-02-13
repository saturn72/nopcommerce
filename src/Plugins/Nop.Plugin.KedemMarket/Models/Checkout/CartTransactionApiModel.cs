using KedemMarket.Common.Models.Cart;
using KedemMarket.Models.Directory;

namespace KedemMarket.Api.Models.Checkout;

public record CartTransactionApiModel
{
    public IEnumerable<ShoppingCartItemApiModel> Items { get; init; }
    public string PaymentMethod { get; init; }
    public string Status { get; init; }
    public bool StorePickup { get; init; }
    public ContactInfoModel BillingInfo { get; init; }
    public ContactInfoModel ShippingInfo { get; init; }
    public bool SubscribeToEMarketing { get; init; }
    public bool UpdateBillingInfo { get; set; }
    public bool UpdateShippingInfo { get; set; }
}
