namespace KedemMarket.Admin.Models.Navbar;

public record AddVendorToNavbarElementSearchModel : BaseSearchModel
{
    public int NavbarElementId { get; set; }
    public int SearchVendorId { get; set; }
    public string SearchVendorName { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; } = new List<SelectListItem>();
    public IList<int> SelectedVendorIds { get; set; } = new List<int>();
}