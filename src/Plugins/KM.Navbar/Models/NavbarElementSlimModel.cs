
namespace KM.Navbar.Models;
public class NavbarElementSlimModel
{
    public string Alt { get; init; }
    public string Icon { get; init; }
    public string ActiveIcon { get; init; }
    public int Index { get; init; }
    public string Caption { get; init; }
    public string Tags { get; init; } = string.Empty;
    public string Type { get; init; }
    public string Value { get; init; }
}
