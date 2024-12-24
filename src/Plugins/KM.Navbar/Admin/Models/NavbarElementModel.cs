namespace KM.Navbar.Admin.Models;

public record NavbarElementModel : BaseNopEntityModel
{
    public int NavbarInfoId { get; init; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Alt")]
    public string Alt { get; init; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Icon")]
    public string Icon { get; init; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Index")]
    public int Index { get; init; }
    [NopResourceDisplayName("Admin.NaNavbarElement.Fields.Label")]
    public string Label { get; init; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Tags")]
    public string Tags { get; init; } = string.Empty;
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Type")]
    public string Type { get; init; }
    [NopResourceDisplayName("Admin.NavbarElement.Fields.Value")]
    public string Value { get; init; }
}
