namespace KM.Orders.Models.Checkout
{
    public record CartTransactionItemApiModel
    {
        public int ProductId { get; init; }
        public int OrderedQuantity { get; init; }
        public decimal CustomerEnteredPrice { get; init; }
    }

}
