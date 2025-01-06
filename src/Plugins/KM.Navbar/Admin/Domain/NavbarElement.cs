namespace KM.Navbar.Admin.Domain;

public class NavbarElement : BaseEntity
{
    public int NavbarInfoId { get; init; }
    public string ActiveIcon { get; init; }
    public string Alt { get; init; }
    public string Caption { get; init; }
    public string Icon { get; init; }
    public int Index { get; init; }
    public string Tags { get; init; }
    public string Type { get; init; }
    public string Value { get; init; }
    public IList<NavbarElementVendor> Vendors { get; init; }
}
