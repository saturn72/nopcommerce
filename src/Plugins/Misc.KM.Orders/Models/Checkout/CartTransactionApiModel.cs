namespace Nop.Plugin.Misc.KM.Orders.Models.Checkout
{
    public record CartTransactionApiModel
    {
        public IEnumerable<CartTransactionItemApiModel> Items { get; init; }
        public string PaymentMethod { get; init; }
        public string Status { get; init; }
        public int StoreId { get; init; }
        public string KmOrderId { get; init; }
        public string UserId { get; init; }
    }
}
