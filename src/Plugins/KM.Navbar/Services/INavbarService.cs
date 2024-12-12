using KM.Navbar.Domain;

namespace KM.Navbar.Services;
public interface INavbarService
{
    Task<NavbarInfo> GetNavbarInfoAsync();
}
