using Nop.Web.Framework.Models;

namespace KM.Navbar.Admin.Models;
public record NavbarInfoModel : BaseNopEntityModel
{
    public string Name { get; set; }
    public IList<NavbarElementModel> Elements { get; set; }
    public int Index { get; set; }
    public bool Published { get; set; }
}
