namespace KM.Navbar.Admin.Models;
public record CreateOrUpdateNavbarElementModel : BaseNopEntityModel
{
    public int NavbarInfoId { get; set; }
    public int NavbarElementId { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.ActiveIcon")]
    public string ActiveIcon { get; init; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Alt")]
    public string Alt { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Caption")]
    public string Caption { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Icon")]
    public string Icon { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Index")]
    public int Index { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Tags")]
    public string Tags { get; set; } = string.Empty;
    [NopResourceDisplayName("Admin.NavbarElement.Fields.AvailableTypes")]
    public IList<SelectListItem> AvailableTypes { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Type")]
    public string Type { get; set; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Value")]
    public string Value { get; set; }

    [NopResourceDisplayName("Admin.NavbarElement.Fields.Vendors")]
    public NavbarElementVendorListSearchModel VendorSearchModel { get; set; }
}