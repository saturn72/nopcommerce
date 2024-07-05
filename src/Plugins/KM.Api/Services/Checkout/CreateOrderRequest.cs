namespace KM.Api.Services.Checkout;
public record CreateOrderRequest
{
    public string KmUserId { get; init; }
    public int StoreId { get; set; }
    public IEnumerable<ShoppingCartItem> CartItems { get; init; }
    public string PaymentMethod { get; init; }
    public bool StorePickup { get;init; }
    public Address BillingInfo { get; init; }
    public Address ShippingAddress { get; init; }
}
