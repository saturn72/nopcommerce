
namespace Km.Api.Controllers
{
    [ApiController]
    [Area("api")]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly IRateLimiter _rateLimiter;
        private readonly IKmOrderService _kmOrderService;

        public CheckoutController(
            IRateLimiter rateLimiter,
            IKmOrderService kmOrderService)
        {
            _rateLimiter = rateLimiter;
            _kmOrderService = kmOrderService;
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
                KmOrderId = model.KmOrderId,
                KmUserId = model.UserId,
                StoreId = model.StoreId,
                CartItems = toCartItems(),
                PaymentMethod = model.PaymentMethod.ToSystemPaymentMethod(),
            };

            var created = await _kmOrderService.CreateOrdersAsync(new[] { co });
            var failed = created.First().IsError;
            return failed ? BadRequest() : Accepted();

            IEnumerable<ShoppingCartItem> toCartItems() =>
                model.Items.Select(s => new ShoppingCartItem { ProductId = s.ProductId, Quantity = s.OrderedQuantity, CustomerEnteredPrice = s.CustomerEnteredPrice });
        }
    }
}
