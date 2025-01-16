
using KM.Navbar.Models;

namespace KM.Navbar.Factories;
public interface INavbarFactory
{
    Task<NavbarAppModel?> PrepareNavbarApiModelByNameAsync(string name);
}
