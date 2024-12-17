

namespace KM.Navbar.Admin.Models;
public record NavbarInfoModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.Navbar.Fields.Name")]
    public string Name { get; set; }
    [NopResourceDisplayName("Admin.Navbar.Fields.Elements")]
    public IList<NavbarElementModel> Elements { get; set; }
    [NopResourceDisplayName("Admin.Navbar.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }
    [NopResourceDisplayName("Admin.Navbar.Fields.Published")]
    public bool Published { get; set; }
}
