
namespace KM.Navbar.Services;
public interface INavbarInfoService
{
    Task DeleteNavbarInfosAsync(IEnumerable<NavbarInfo> navbars);
    Task<IPagedList<NavbarInfo>> GetAllNavbarInfosAsync(string navbarName, bool showHidden, int storeId, int pageIndex, int pageSize, bool? overridePublished);
    Task<IPagedList<NavbarElement>> GetNavbarElementsByNavbarInfoIdAsync(int navbarInfoId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<NavbarInfo> GetNavbarInfoByIdAsync(int id);
    Task<IEnumerable<NavbarInfo>> GetNavbarInfoByIdsAsync(IEnumerable<int> ids);
    Task<NavbarInfo> GetNavbarInfoByNameAsync(string name);
    Task InsertNavbarAsync(NavbarInfo navbar);
    Task UpdateNavbarInfoAsync(NavbarInfo navbar);
}
