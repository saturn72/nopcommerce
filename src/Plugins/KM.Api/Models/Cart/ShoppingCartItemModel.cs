namespace Km.Api.Models.Cart
{
    public record ShoppingCartItemModel
    {
        public int ProductId { get; init; }
        public int OrderedQuantity { get; init; }
    }

}
