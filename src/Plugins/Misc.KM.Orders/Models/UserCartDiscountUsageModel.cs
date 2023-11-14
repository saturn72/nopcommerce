namespace Nop.Plugin.Misc.KM.Orders.Models
{
    public record UserCartDiscountUsageModel
    {
        public int DiscountId { get; set; }
        public string DiscountName { get; set; }
        public string CouponCode { get; set; }
    }
}
