

namespace Km.Orders.Domain.User
{
    public class KmUserCustomerMap : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string KmUserId { get; set; }
        public DateTime CreatedOnUtc { get; init; }
        public string ProviderId { get; set; }
        public string TenantId { get; set; }
        public bool ShouldProvisionBasicClaims { get; set; }
    }
}
