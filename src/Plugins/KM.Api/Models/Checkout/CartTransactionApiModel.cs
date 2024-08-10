namespace KM.Api.Models.Checkout;

public record CartTransactionApiModel
{
    public IEnumerable<CartTransactionItemApiModel> Items { get; init; }
    public string PaymentMethod { get; init; }
    public string Status { get; init; }
    public int StoreId { get; init; }
    public bool StorePickup { get; init; }
    public string UserId { get; init; }
    public ContactInfoModel BillingInfo { get; init; }
    public ContactInfoModel ShippingInfo { get; init; }
    public bool SubscribeToEMarketing { get; init; }
}
