namespace KM.Navbar.Admin.Domain;
public class NavbarInfo : BaseEntity
{
    public IEnumerable<NavbarElement> Elements { get; init; }
    public int Index { get; set; }
    public string Name { get; set; }
    public bool Published { get; set; }
}

