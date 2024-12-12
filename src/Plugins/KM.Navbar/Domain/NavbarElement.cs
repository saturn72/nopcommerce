namespace KM.Navbar.Domain;

public class NavbarElement : BaseEntity
{
    public string Label { get; init; }
    public string Alt { get; init; }
    public string Icon { get; init; }
    public string Tags { get; init; }
    public NavbarType Type { get; init; }
    public string Value { get; init; }
}

