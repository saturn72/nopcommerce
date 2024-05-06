namespace Km.Api.Domains
{
    public record Order
    {
        public Customer Customer { get; set; }
        public string KnUserId { get; set; }
        public IList<ShoppingCartItem> Items { get; set; }

    }
}
