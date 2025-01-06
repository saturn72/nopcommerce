
namespace KM.Navbar.Admin.Models;
public record AddOrRemoveVendorToNavbarElementSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Navbars.Elements.Vendors.List.SearchVendorName")]
    public string SearchVendorName { get; set; }

    [NopResourceDisplayName("Admin.Navbars.Elements.Vendors.List.SearchNavbarElement")]
    public int SearchNavbarElementId { get; set; }
    [NopResourceDisplayName("Admin.Navbars.Elements.Vendors.List.SearchVendorId")]
    public int SearchVendorId { get; set; }

    public IList<int> SelectedVendorIds { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; }= new List<SelectListItem>();
}