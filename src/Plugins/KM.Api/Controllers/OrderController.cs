namespace KM.Api.Controllers;

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
    public async Task<IActionResult> GetOrders([FromQuery]int pageSize = 25, [FromQuery]int offset = 0)
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

        var data = orders == null || orders.Count == 0?
            Array.Empty<OrderInfoModel>():
            await _orderApiModelFactory.PrepareOrderDetailsModelsAsync(orders);
        return ToJsonResult(data);
    }

    [HttpDelete]
    public async Task<IActionResult> CancelOrderByOrderNumber([FromBody]OrderCancellationRequestModel request)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == default)
            return BadRequest();

        var order = await _orderService.GetOrderByIdAsync(request.OrderId);
        if (order == null || order.CustomerId != customer.Id)
            return NotFound();
        throw new NotImplementedException("send message to vendor/seller");
        throw new NotImplementedException("Message should contains -orderID, -order summary, -customer info -cancellation reason");
        //_messagingService
        //await _orderService.DeleteOrderAsync(order);

        //var data = await _orderApiModelFactory.PrepareOrderDetailsModelAsync(order);
        //return ToJsonResult(data);
    }
}

//using KM.Api.Models.Orders;

//namespace KM.Api.Controllers;

//[Route("api/order")]
//public class OrderController : KmApiControllerBase
//{
//    private readonly IWorkContext _workContext;
//    private readonly IOrderService _orderService;
//    private readonly IOrderModelFactory _orderModelFactory;


//    public OrderController(
//        IWorkContext workContext,
//        IOrderService orderService,
//        IOrderModelFactory orderModelFactory)
//    {
//        _workContext = workContext;
//        _orderService = orderService;
//        _orderModelFactory = orderModelFactory;
//    }

//    [HttpGet("{orderNumber}")]
//    public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
//    {
//        var customer = await _workContext.GetCurrentCustomerAsync();
//        if (customer == default)
//            return BadRequest();

//        var order = await _orderService.GetOrderByCustomOrderNumberAsync(orderNumber);
//        if (order.CustomerId != customer.Id)
//            return NotFound();
//        var odm = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);

//        var items = odm.Items.Select(oi => new OrderInfoModel.OrderItem
//        {
//            Id = oi.Id,
//            AttributeInfo = oi.AttributeInfo,
//            OrderItemGuid = oi.OrderItemGuid,
//            Quantity = oi.Quantity,
//            Picture = oi.Picture,
//            ProductId = oi.ProductId,
//            ProductName = oi.ProductName,
//            ProductSeName = oi.ProductSeName,
//            Sku = oi.Sku,
//            SubTotal = oi.SubTotal,
//            SubTotalValue = oi.SubTotalValue,
//            UnitPrice = oi.UnitPrice,
//            UnitPriceValue = oi.UnitPriceValue,
//            VendorName = oi.VendorName,
//        }).ToList();

//        var o = new OrderInfoModel
//        {
//            Id = odm.Id,
//            BillingAddress = odm.BillingAddress.ToContactInfoModel(),
//            CreatedOnUtc = odm.CreatedOn,
//            CustomOrderNumber = odm.CustomOrderNumber,
//            Items = items,
//        };

//        return ToJsonResult(o);
//    }
//}
