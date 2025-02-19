namespace KedemMarket.Factories.Navbar;
public interface INavbarFactory
{
    Task<NavbarAppModel> PrepareNavbarApiModelByNameAsync(string name);
}
