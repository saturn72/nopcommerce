using KM.Api.Models.OrderManagement;
using Nop.Services.Media;
using Nop.Web.Models.Order;

namespace KM.Api.Factories;

public class OrderApiModelFactory : IOrderApiModelFactory
{
    private readonly IOrderModelFactory _orderModelFactory;
    private readonly IPictureService _pictureService;
    private readonly MediaConvertor _mediaConvertor;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IExternalUsersService _extenrnalUserService;

    public OrderApiModelFactory(
        IOrderModelFactory orderModelFactory,
        IPictureService pictureService,
        MediaConvertor mediaConvertor,
        IOrderService orderService,
        IProductService productService,
        IExternalUsersService extenrnalUserService)
    {
        _orderModelFactory = orderModelFactory;
        _pictureService = pictureService;
        _mediaConvertor = mediaConvertor;
        _orderService = orderService;
        _productService = productService;
        _extenrnalUserService = extenrnalUserService;
    }

    public async Task<IEnumerable<OrderInfoModel>> PrepareOrderDetailsModelsAsync(IEnumerable<Order> orders)
    {
        return await orders.SelectAwait(async o => await PrepareOrderDetailsModelAsync(o)).ToListAsync();
    }
    public async Task<OrderInfoModel> PrepareOrderDetailsModelAsync(Order order)
    {

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
        var odm = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);

        var items = new List<OrderInfoModel.OrderItem>();
        foreach (var i in odm.Items)
        {
            var oi = orderItems.FirstOrDefault(o => o.Id == i.Id);
            var item = await toOrderItemAsync(i, oi);
            items.Add(item);
        }

        var shipments = odm.Shipments.Select(s =>
        {
            return new OrderInfoModel.ShipmentBriefModel
            {
                TrackingNumber = s.TrackingNumber,
                ShippedDate = s.ShippedDate,
                ReadyForPickupDate = s.ReadyForPickupDate,
                DeliveryDate = s.DeliveryDate,

            };
        }).ToList();

        var orderNotes = odm.OrderNotes.Select(n => new OrderInfoModel.OrderNote
        {
            CreatedOnUtc = n.CreatedOn,
            HasDownload = n.HasDownload,
            Note = n.Note,
        });

        var extUser = await _extenrnalUserService.GetUserIdCustomerMapByInternalCustomerId(order.CustomerId);

        return new OrderInfoModel
        {
            Id = odm.Id,
            UserId = extUser.KmUserId,
            BillingAddress = odm.BillingAddress.ToContactInfoModel(),
            CreatedOnUtc = odm.CreatedOn,
            CustomOrderNumber = odm.CustomOrderNumber,
            Items = items,
            OrderNotes = orderNotes,
            OrderShipping = odm.OrderShipping,
            OrderShippingValue = odm.OrderShippingValue,
            OrderStatus = odm.OrderStatus.ToLower(),
            OrderSubtotal = odm.OrderSubtotal,
            OrderSubtotalValue = odm.OrderSubtotalValue,
            OrderSubTotalDiscount = odm.OrderSubTotalDiscount,
            OrderSubTotalDiscountValue = odm.OrderSubTotalDiscountValue,
            PaidOnUtc = order.PaidDateUtc,
            PaymentMethod = odm.PaymentMethod,
            PaymentMethodAdditionalFee = odm.PaymentMethodAdditionalFee,
            PaymentMethodAdditionalFeeValue = odm.PaymentMethodAdditionalFeeValue,
            PaymentMethodStatus = odm.PaymentMethodStatus.ToLower(),
            PickupAddress = odm.PickupAddress.ToAddressApiModel(),
            Pickup = odm.PickupInStore,
            ShippingAddress = odm.ShippingAddress.ToContactInfoModel(),
            ShippingMethod = odm.ShippingMethod,
            ShippingStatus = odm.ShippingStatus.ToLower(),
            Shipments = shipments,
        };

        async Task<OrderInfoModel.OrderItem> toOrderItemAsync(OrderDetailsModel.OrderItemModel omi, OrderItem orderItem)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
            var orderItemPicture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
            var picture = orderItemPicture != default ? await _mediaConvertor.ToGalleryItemModel(orderItemPicture, 0) : default;

            //order item picture
            return new OrderInfoModel.OrderItem
            {
                Id = omi.Id,
                AttributeInfo = omi.AttributeInfo,
                OrderItemGuid = omi.OrderItemGuid,
                Quantity = omi.Quantity,
                Picture = picture,
                ProductId = omi.ProductId,
                ProductName = omi.ProductName,
                ProductSeName = omi.ProductSeName,
                Sku = omi.Sku,
                SubTotal = omi.SubTotal,
                SubTotalValue = omi.SubTotalValue,
                UnitPrice = omi.UnitPrice,
                UnitPriceValue = omi.UnitPriceValue,
                VendorName = omi.VendorName,
            };
        }
    }
}
