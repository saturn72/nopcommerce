namespace KM.Navbar.Admin.Models;

public record NavElementVendorModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.VendorName")]
    public string VendorName { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.IsFeaturedVendor")]
    public bool IsFeaturedVendor { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }
    public int NavbarElementId { get; set; }
    public int VendorId { get; set; }
}
