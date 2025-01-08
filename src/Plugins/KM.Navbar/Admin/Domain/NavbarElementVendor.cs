using Nop.Core.Domain.Vendors;

namespace KM.Navbar.Admin.Domain;

public class NavbarElementVendor : BaseEntity
{
    public int DisplayOrder { get; init; }
    public bool IsFeaturedVendor { get; set; }
    public int NavbarElementId { get; init; }
    public bool Published { get; init; }
    public int VendorId { get; init; }

    public Vendor Vendor { get; set; }
    public NavbarElement NavbarElement { get; init; }
}
