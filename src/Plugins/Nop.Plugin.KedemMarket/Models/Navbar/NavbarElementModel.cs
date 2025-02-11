namespace KedemMarket.Models.Navbar;
public class NavbarElementModel
{
    public string Alt { get; init; }
    public string Icon { get; init; }
    public string ActiveIcon { get; init; }
    public int Index { get; init; }
    public string Caption { get; init; }
    public string Tags { get; init; } = string.Empty;
    public string Type { get; init; }
    public string Value { get; init; }
    public IEnumerable<VendorModel> Vendors { get; init; } = new List<VendorModel>();
}
