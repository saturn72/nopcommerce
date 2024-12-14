namespace KM.Navbar.Admin.Domain;

public class NavbarElement : BaseEntity
{
    public string Alt { get; init; }
    public string Icon { get; init; }
    public int Index { get; init; }
    public string Label { get; init; }
    public string Tags { get; init; }
    public NavbarType Type { get; init; }
    public string Value { get; init; }
    public int NavbarInfoId { get; init; }
}

