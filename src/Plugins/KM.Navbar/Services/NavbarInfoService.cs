namespace KM.Navbar.Services;

public class NavbarInfoService : INavbarInfoService
{
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IRepository<NavbarInfo> _navbarInfoRepository;
    private readonly IRepository<NavbarElement> _navbarElementRepository;
    private readonly IStoreMappingService _storeMappingService;
    private const int CACHE_TIME = 30 * 24 * 60;
    private static readonly CacheKey _cacheKey = new("navbars")
    {
        CacheTime = CACHE_TIME
    };

    public NavbarInfoService(
        IRepository<NavbarInfo> navbarInfoRepository,
        IRepository<NavbarElement> navbarElementRepository,
        IStaticCacheManager staticCacheManager
,
        IStoreMappingService storeMappingService)
    {
        _navbarInfoRepository = navbarInfoRepository;
        _navbarElementRepository = navbarElementRepository;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
    }

    public async Task<NavbarInfo> GetNavbarInfoAsync()
    {
        return await _staticCacheManager.GetAsync(_cacheKey, async () =>
        {
            var e = await _navbarElementRepository.Table.ToListAsync();
            return new NavbarInfo { Elements = e };
        });
    }

    public async Task<IPagedList<NavbarInfo>> GetAllNavbarInfosAsync(string navbarName, bool showHidden, int storeId, int pageIndex, int pageSize, bool? overridePublished)
    {
        var navbarInfos = await _navbarInfoRepository.GetAllAsync(async query =>
        {
            if (!showHidden)
                query = query.Where(c => c.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!string.IsNullOrWhiteSpace(navbarName))
                query = query.Where(c => c.Name.Contains(navbarName));

            return query.Where(c => !c.Deleted);
        });

        return new PagedList<NavbarInfo>(navbarInfos, pageIndex, pageSize);
    }

    public async Task<bool> InsertNavbarAsync(NavbarInfo navbar)
    {
        var allNavbars = _navbarInfoRepository.GetAll(
            query => query.Where(c => !c.Deleted && c.Name.Equals(navbar.Name, StringComparison.InvariantCultureIgnoreCase)),
            cache => new CacheKey(_cacheKey.Key, [navbar.Name])
            {
                CacheTime = CACHE_TIME
            });

        if (allNavbars.Any())
            return false;

        await _navbarInfoRepository.InsertAsync(navbar);
        return navbar.Id != 0;
    }
}
