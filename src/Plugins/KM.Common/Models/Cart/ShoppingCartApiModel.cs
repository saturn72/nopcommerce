namespace KedemMarket.Common.Models.Cart;

public record ShoppingCartApiModel
{
    public IEnumerable<ShoppingCartItemApiModel>? Items { get; init; }
    public long UpdatedOnUtc { get; init; }
}

