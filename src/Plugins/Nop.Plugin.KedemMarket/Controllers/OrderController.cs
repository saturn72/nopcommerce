namespace KedemMarket.Controllers;

[Route("api/order")]
public class OrderController : KmApiControllerBase
{
    private readonly IWorkContext _workContext;
    private readonly IOrderService _orderService;
    private readonly IOrderApiModelFactory _orderApiModelFactory;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;

    public OrderController(
        IWorkContext workContext,
        IOrderService orderService,
        IOrderApiModelFactory orderApiModelFactory,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext)
    {
        _workContext = workContext;
        _orderService = orderService;
        _orderApiModelFactory = orderApiModelFactory;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderByOrderNumber(int orderId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == default)
            return BadRequest();

        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null || order.CustomerId != customer.Id)
            return NotFound();

        var data = await _orderApiModelFactory.PrepareOrderDetailsModelAsync(order);
        return ToJsonResult(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int pageSize = 25, [FromQuery] int offset = 0)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == default)
            return BadRequest();

        var store = await _storeContext.GetCurrentStoreAsync();
        var orders = await _orderService.SearchOrdersAsync(
            storeId: store.Id,
            customerId: customer.Id,
            pageIndex: offset / pageSize,
            pageSize: pageSize);

        var data = orders.NotNullAndNotNotEmpty() ? await _orderApiModelFactory.PrepareOrderDetailsModelsAsync(orders) : [];
        return ToJsonResult(data);
    }

    [HttpDelete]
    public async Task<IActionResult> CancelOrderByOrderNumber([FromBody] OrderCancellationRequestModel request)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == default)
            return BadRequest();

        var order = await _orderService.GetOrderByIdAsync(request.OrderId);
        if (order == null || order.CustomerId != customer.Id)
            return NotFound();
        throw new NotImplementedException("send message to vendor/seller");
        throw new NotImplementedException("Message should contains -orderID, -order summary, -customer info -cancellation reason");
    }
}