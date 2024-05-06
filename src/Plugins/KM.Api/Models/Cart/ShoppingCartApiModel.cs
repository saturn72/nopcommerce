namespace Km.Api.Models.Cart
{
    public record ShoppingCartApiModel
    {
        public int StoreId { get; init; }
        public string UserId { get; init; }
        public IEnumerable<CartTransactionItemApiModel> Items { get; init; }
    }

}
