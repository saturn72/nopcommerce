namespace Nop.Plugin.Misc.KM.Orders.Services.Checkout
{
    public class CreateOrderResponse
    {
        public CreateOrderRequest Request { get; init; }
        public IEnumerable<ShoppingCartItem> ApprovedShoppingCartItems { get; set; }
        public IEnumerable<ShoppingCartItem> DisapprovedShoppingCartItems { get; set; }
        public bool IsError => Error.HasValue();
        public string Error { get; set; }
    }
}
