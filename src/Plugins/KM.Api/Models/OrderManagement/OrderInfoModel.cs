using KedemMarket.Api.Models.Directory;
using KedemMarket.Common.Models.Media;

namespace KedemMarket.Api.Models.OrderManagement;

public record OrderInfoModel
{
    public int Id { get; init; }
    public ContactInfoModel BillingAddress { get; init; }
    public string CustomOrderNumber { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public IEnumerable<OrderItem> Items { get; init; }
    public string OrderTotal { get; init; }
    public decimal OrderTotalValue { get; init; }
    public string OrderTotalDiscount { get; init; }
    public decimal OrderTotalDiscountValue { get; init; }
    public string OrderShipping { get; init; }
    public decimal OrderShippingValue { get; init; }
    public string OrderStatus { get; init; }
    public string OrderSubtotal { get; init; }
    public decimal OrderSubtotalValue { get; init; }
    public string OrderSubTotalDiscount { get; init; }
    public decimal OrderSubTotalDiscountValue { get; init; }
    public string UserId { get; init; }
    //payment
    public DateTime? PaidOnUtc { get; init; }
    public string PaymentMethod { get; init; }
    public string PaymentMethodAdditionalFee { get; init; }
    public decimal PaymentMethodAdditionalFeeValue { get; init; }
    public string PaymentMethodStatus { get; init; }

    //shipping/pickup
    public bool Pickup { get; init; }
    public AddressApiModel PickupAddress { get; init; }
    public ContactInfoModel ShippingAddress { get; init; }
    public string ShippingMethod { get; init; }
    public string ShippingStatus { get; init; }
    public IEnumerable<ShipmentBriefModel> Shipments { get; init; }
    public IEnumerable<OrderNote> OrderNotes { get; init; }

    #region Nested Classes
    public record OrderItem
    {
        public int Id { get; init; }
        public string AttributeInfo { get; init; }
        public Guid OrderItemGuid { get; init; }
        public int Quantity { get; init; }
        public GalleryItemModel Picture { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public string ProductSeName { get; init; }
        public string Sku { get; init; }
        public string SubTotal { get; init; }
        public decimal SubTotalValue { get; init; }
        public string UnitPrice { get; init; }
        public decimal UnitPriceValue { get; init; }
        public string VendorName { get; init; }
    }
    public record GiftCard
    {
        public int Id { get; init; }
        public string CouponCode { get; init; }
        public string Amount { get; init; }
    }

    public record OrderNote
    {
        public bool HasDownload { get; init; }
        public string Note { get; init; }
        public DateTime CreatedOnUtc { get; init; }
    }

    public record ShipmentBriefModel
    {
        public string TrackingNumber { get; init; }
        public DateTime? ShippedDate { get; init; }
        public DateTime? ReadyForPickupDate { get; init; }
        public DateTime? DeliveryDate { get; init; }
    }

    #endregion
}
