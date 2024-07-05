namespace KM.Api.Controllers;

[Route("api/checkout")]
public class CheckoutController : KmApiControllerBase
{
    private readonly IRateLimiter _rateLimiter;
    private readonly IKmOrderService _kmOrderService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IWorkContext _workContext;

    public CheckoutController(
        IRateLimiter rateLimiter,
        IKmOrderService kmOrderService,
        IShoppingCartService shoppingCartService,
        IWorkContext workContext)
    {
        _rateLimiter = rateLimiter;
        _kmOrderService = kmOrderService;
        _shoppingCartService = shoppingCartService;
        _workContext = workContext;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] CartTransactionApiModel model)
    {
        //block user from submitting 2 orders in 2000 milisec timeframe
        var v = await _rateLimiter.Limit($"cart-transfer-{model.UserId}", 2000);
        if (!v)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest();

        var co = new CreateOrderRequest
        {
            //KmOrderId = model.KmOrderId,
            KmUserId = model.UserId,
            StoreId = model.StoreId,
            CartItems = toCartItems(),
            PaymentMethod = model.PaymentMethod.ToSystemPaymentMethod(),
            StorePickup = model.StorePickup,
        };

        var created = await _kmOrderService.CreateOrdersAsync(new[] { co });
        var c = created.First();
        if (c.IsError)
            return BadRequest(new { error = c.Error });

        await clearCustomerCart();
        return Accepted();

        async Task clearCustomerCart()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _shoppingCartService.ClearShoppingCartAsync(customer, model.StoreId);
        }

        IEnumerable<ShoppingCartItem> toCartItems() =>
            model.Items.Select(s => new ShoppingCartItem { ProductId = s.ProductId, Quantity = s.OrderedQuantity, CustomerEnteredPrice = s.CustomerEnteredPrice });
    }
}
