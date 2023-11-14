namespace Nop.Plugin.Misc.KM.Orders.Domains
{
    public record Order
    {
        public Customer Customer { get; set; }
        public string KnUserId { get; set; }
        public IList<ShoppingCartItem> Items { get; set; }

    }
}
