namespace KedemMarket.Admin.Models.Navbar;

public record NavbarElementVendorSelectModel : BaseNopModel
{
    public int VendorId { get; set; }
    public string VendorName { get; set; }
}