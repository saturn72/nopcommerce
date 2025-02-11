namespace KedemMarket.Admin.Models;

public record VendorAddPopupListModel : BasePagedListModel<NavbarElementVendorSelectModel>
{
    public IList<int> SelectedVendorIds { get; set; } = new List<int>();
}
