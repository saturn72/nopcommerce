namespace KM.Api.Models.Cart;

public record ShoppingCartApiModel
{
    public IEnumerable<ShoppingCartItemApiModel> Items { get; init; }
    public long UpdatedOnUtc { get; init; }
}

