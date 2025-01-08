namespace KM.Navbar.Admin.Models;

public record NavbarElementVendorListModel : BasePagedListModel<NavbarElementVendorModel>
{
    public int NavbarElementId { get; set; }
}
