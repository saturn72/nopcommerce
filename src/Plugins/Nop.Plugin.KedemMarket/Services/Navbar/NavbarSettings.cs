using Nop.Core.Configuration;

namespace KedemMarket.Services.Navbar;
public class NavbarSettings : ISettings
{
    public string PageSizeOptions { get; set; } = "10, 25, 50, 100";
    public int DefaultPageSize { get; set; } = 25;
}
