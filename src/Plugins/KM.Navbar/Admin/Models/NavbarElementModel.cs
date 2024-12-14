using Nop.Web.Framework.Models;

namespace KM.Navbar.Admin.Models;

public record NavbarElementModel : BaseNopEntityModel
{
    public string Alt { get; init; }
    public string Icon { get; init; }
    public int Index { get; init; }
    public string Label { get; init; }
    public string Tags { get; init; }
    public string Type { get; init; }
    public string Value { get; init; }
}
