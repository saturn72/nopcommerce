namespace KM.Api.Controllers;

[Route("api/order")]
public class OrderController : KmApiControllerBase
{
    private readonly IWorkContext _workContext;
    private readonly IOrderService _orderService;
    private readonly IOrderApiModelFactory _orderApiModelFactory;
    private readonly IStaticCacheManager _staticCacheManager;
    public OrderController(
        IWorkContext workContext,
        IOrderService orderService,
        IOrderApiModelFactory orderApiModelFactory,
        IStaticCacheManager staticCacheManager)
    {
        _workContext = workContext;
        _orderService = orderService;
        _orderApiModelFactory = orderApiModelFactory;
        _staticCacheManager = staticCacheManager;
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
