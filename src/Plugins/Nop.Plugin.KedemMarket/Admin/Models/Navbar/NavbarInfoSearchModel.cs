namespace KedemMarket.Admin.Models.Navbar;
public record NavbarInfoSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Navbars.List.SearchNavbarName")]
    public string SearchNavbarName { get; set; }
    [NopResourceDisplayName("Admin.Navbars.List.SearchPublished")]
    public int SearchPublishedId { get; set; }
    public IList<SelectListItem> AvailablePublishedOptions { get; set; } = new List<SelectListItem>();
    [NopResourceDisplayName("Admin.Navbars.List.SearchStore")]
    public int SearchStoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; } = new List<SelectListItem>();
    public bool HideStoresList { get; set; }
}

