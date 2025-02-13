namespace KedemMarket.Admin.Models.Navbar;

public partial record NavbarElementSearchModel : BaseSearchModel
{
    public int NavbarInfoId { get; set; }
    //[NopResourceDisplayName("Admin.Navbars.Fields.PageSize")]
    [NopResourceDisplayName("Admin.Navbars.Fields.AllowCustomersToSelectPageSize")]
    public bool AllowCustomersToSelectPageSize { get; set; }

    [NopResourceDisplayName("Admin.Navbars.Fields.PageSizeOptions")]
    public string PageSizeOptions { get; set; }
}