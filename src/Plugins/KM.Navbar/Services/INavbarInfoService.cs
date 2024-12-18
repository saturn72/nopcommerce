
namespace KM.Navbar.Services;
public interface INavbarInfoService
{
    Task<IPagedList<NavbarInfo>> GetAllNavbarInfosAsync(string navbarName, bool showHidden, int storeId, int pageIndex, int pageSize, bool? overridePublished);
    Task<NavbarInfo> GetNavbarInfoAsync();
    Task<bool> InsertNavbarAsync(NavbarInfo navbar);
}
