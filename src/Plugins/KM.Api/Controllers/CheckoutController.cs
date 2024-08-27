using KM.Api.Factories;

namespace KM.Api.Controllers;

[Route("api/checkout")]
public class CheckoutController : KmApiControllerBase
{
    private readonly IKmOrderService _kmOrderService;
    private readonly IShoppingCartFactory _shoppingCartFactory;

    public CheckoutController(
        IKmOrderService kmOrderService,
        IShoppingCartFactory shoppingCartFactory)
    {
        _kmOrderService = kmOrderService;
        _shoppingCartFactory = shoppingCartFactory;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] CartTransactionApiModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var errors = new List<string>();
        var cor = await _shoppingCartFactory.ToCreateOrderRequest(model, errors);

        var created = await _kmOrderService.CreateOrderAsync(cor);
        if (created.IsError)
            return BadRequest(new { error = created.Error });

        return Accepted();
    }
}
