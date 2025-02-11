namespace KedemMarket.Admin.Models;

public record NavbarElementVendorModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.IsFeaturedVendor")]
    public bool IsFeaturedVendor { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.VendorName")]
    public string VendorName { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Vendors.Fields.Published")]
    public bool Published { get; set; }
    public int NavbarElementId { get; set; }
    public int VendorId { get; set; }
}
