namespace KedemMarket.Admin.Domain;
public class NavbarElementVendor : BaseEntity
{
    public int DisplayOrder { get; set; }
    public bool IsFeaturedVendor { get; set; }
    public int NavbarElementId { get; set; }
    public bool Published { get; set; }
    public int VendorId { get; set; }

    public Vendor Vendor { get; set; }
    public NavbarElement NavbarElement { get; set; }
}
