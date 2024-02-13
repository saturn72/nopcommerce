namespace Km.Orders.Services.Checkout;
public record CreateOrderRequest
{
    public string KmOrderId { get; init; }
    public string KmUserId { get; init; }
    public int StoreId { get; set; }
    public IEnumerable<ShoppingCartItem> CartItems { get; init; }
    public string PaymentMethod { get; init; }
    public Address BillingInfo { get; init; }
    public Address ShippingAddress { get; init; }
}
