namespace KedemMarket.Admin.Models.Navbar;

public record NavbarElementVendorListModel : BasePagedListModel<NavbarElementVendorModel>
{
    public int NavbarElementId { get; set; }
}
