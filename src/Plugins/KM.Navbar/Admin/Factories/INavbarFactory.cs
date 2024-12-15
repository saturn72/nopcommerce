namespace KM.Navbar.Admin.Factories;
public interface INavbarFactory
{
    Task<NavbarInfoSearchModel> PrepareNavbarListModelAsync();
}
