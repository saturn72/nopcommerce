namespace KM.Api.Models.Cart;

public record ShoppingCartApiModel
{
    public int StoreId { get; init; }
    public string UserId { get; init; }
    public IEnumerable<ShoppingCartItemApiModel> Items { get; init; }
}

