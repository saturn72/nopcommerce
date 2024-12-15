namespace KM.Navbar.Services;
public interface INavbarService
{
    Task<NavbarInfo> GetNavbarInfoAsync();
}
