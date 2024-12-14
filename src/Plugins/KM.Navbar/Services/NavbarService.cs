using KM.Navbar.Admin.Domain;
using Nop.Core.Caching;
using Nop.Data;

namespace KM.Navbar.Services;

public class NavbarService : INavbarService
{
    private readonly IStaticCacheManager _cacheManager;
    private readonly IRepository<NavbarElement> _navbarRepository;
    private static readonly CacheKey Key = new("navbar", "navbar")
    {
        CacheTime = 30 * 24 * 60
    };

    public NavbarService(
        IStaticCacheManager cacheManager,
        IRepository<NavbarElement> navbarRepository)
    {
        _cacheManager = cacheManager;
        _navbarRepository = navbarRepository;
    }

    public async Task<NavbarInfo> GetNavbarInfoAsync()
    {
        return await _cacheManager.GetAsync(Key, async () =>
        {
            var e = await _navbarRepository.Table.ToListAsync();
            return new NavbarInfo { Elements = e };
        });
    }
}
