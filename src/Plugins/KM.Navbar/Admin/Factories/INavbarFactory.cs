namespace KM.Navbar.Admin.Factories;
public interface INavbarFactory
{
    Task<IEnumerable<NavbarInfoModel>> PrepareNavbarListAsync();
}
