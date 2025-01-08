namespace KM.Navbar.Admin.Models;

public record NavbarElementVendorSelectModel : BaseNopModel
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
}