namespace KM.Api.Domain.Checkout
{
    public class KmOrder : BaseEntity
    {
        public DateTime? CreatedOnUtc { get; init; }
        public string Data { get; init; }
        public string Status { get; init; }
        //public string KmOrderId { get; init; }
        public string KmUserId { get; init; }
        public int NopOrderId { get; init; }
        public Order NopOrder { get; init; }
        public string Errors { get; init; }
    }
}
