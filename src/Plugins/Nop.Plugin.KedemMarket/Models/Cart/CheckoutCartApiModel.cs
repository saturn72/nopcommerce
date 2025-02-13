namespace KedemMarket.Models.Cart;

public record CheckoutCartApiModel
{
    public IEnumerable<CheckoutCartItemApiModel> Items { get; init; }
}
