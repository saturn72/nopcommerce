namespace KM.Api.Models.Cart;

public record ShoppingCartItemApiModel
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

