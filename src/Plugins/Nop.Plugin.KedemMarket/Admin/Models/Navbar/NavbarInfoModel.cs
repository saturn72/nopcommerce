namespace KedemMarket.Admin.Models.Navbar;
public partial record NavbarInfoModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.Navbar.Fields.Elements")]
    public IList<NavbarElementModel> Elements { get; set; }
    [NopResourceDisplayName("Admin.Navbar.Fields.Name")]
    public string Name { get; set; }
    [NopResourceDisplayName("Admin.Navbars.Fields.Description")]
    public string Description { get; set; }
    [NopResourceDisplayName("Admin.Navbar.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }
    [NopResourceDisplayName("Admin.Navbar.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Navbars.Fields.Deleted")]
    public bool Deleted { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Navbars.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; } = new List<int>();
    public IList<SelectListItem> AvailableStores { get; set; } = new List<SelectListItem>();

    public NavbarElementSearchModel NavbarElementSearchModel { get; set; } = new();
}
