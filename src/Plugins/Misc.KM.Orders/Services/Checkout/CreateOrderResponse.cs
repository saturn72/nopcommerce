namespace Nop.Plugin.Misc.KM.Orders.Services.Checkout
{
    public class CreateOrderResponse
    {
        public CreateOrderRequest Request { get; init; }
        public bool IsError => Error.HasValue();
        public string Error { get; set; }
    }
}
