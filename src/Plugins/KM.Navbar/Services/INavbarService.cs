using KM.Navbar.Admin.Domain;

namespace KM.Navbar.Services;
public interface INavbarService
{
    Task<NavbarInfo> GetNavbarInfoAsync();
}
