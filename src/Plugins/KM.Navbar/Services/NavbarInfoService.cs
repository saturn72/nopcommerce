namespace KM.Navbar.Services;

public class NavbarInfoService : INavbarInfoService
{
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IRepository<NavbarInfo> _navbarInfoRepository;
    private readonly IRepository<NavbarElement> _navbarElementRepository;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private const int CACHE_TIME = 30 * 24 * 60;
    private const string CACHE_KEY = "navbars";

    public NavbarInfoService(
        IRepository<NavbarInfo> navbarInfoRepository,
        IRepository<NavbarElement> navbarElementRepository,
        IStaticCacheManager staticCacheManager
,
        IStoreMappingService storeMappingService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _navbarInfoRepository = navbarInfoRepository;
        _navbarElementRepository = navbarElementRepository;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _storeContext = storeContext;
        _workContext = workContext;
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

    public async Task InsertNavbarInfoAsync(NavbarInfo navbarInfo)
    {
        ThrowIfNull(navbarInfo, nameof(navbarInfo));

        var n = navbarInfo.Name.ToLower();
        var allNavbars = _navbarInfoRepository.GetAll(
            query => query.Where(c => !c.Deleted && c.Name.ToLower() == n),
            cache => new CacheKey(CACHE_KEY, [navbarInfo.Name])
            {
                CacheTime = CACHE_TIME
            });

        if (allNavbars.Any())
            return;

        await _navbarInfoRepository.InsertAsync(navbarInfo);
        await _staticCacheManager.RemoveByPrefixAsync(CACHE_KEY);
    }

    public async Task<NavbarInfo> GetNavbarInfoByIdAsync(int id)
    {
        return await _navbarInfoRepository.GetByIdAsync(id, cks =>
        new CacheKey($"{CACHE_KEY}.{id}", [CACHE_KEY])
        {
            CacheTime = CACHE_TIME
        });
    }

    public async Task<NavbarInfo> GetNavbarInfoByNameAsync(string name)
    {
        var ck = new CacheKey($"{CACHE_KEY}.{name}", [CACHE_KEY])
        {
            CacheTime = CACHE_TIME
        };

        return await _staticCacheManager.GetAsync(ck, async () => await _navbarInfoRepository.Table.FirstOrDefaultAsync(c => c.Name == name));
    }

    public async Task UpdateNavbarInfoAsync(NavbarInfo navbarInfo)
    {
        ThrowIfNull(navbarInfo, nameof(navbarInfo));
        await _navbarInfoRepository.UpdateAsync(navbarInfo);
        await _staticCacheManager.RemoveByPrefixAsync(CACHE_KEY);
    }

    public async Task DeleteNavbarInfosAsync(IEnumerable<NavbarInfo> navbarInfoss)
    {
        ThrowIfNull(navbarInfoss, nameof(navbarInfoss));
        if(!navbarInfoss.Any())
            return; 
        await _navbarInfoRepository.DeleteAsync(navbarInfoss.ToList());
        await _staticCacheManager.RemoveByPrefixAsync(CACHE_KEY);
    }

    public async Task<IEnumerable<NavbarInfo>> GetNavbarInfoByIdsAsync(IEnumerable<int> navbarInfoIds)
    {
        return await _navbarInfoRepository.GetByIdsAsync(navbarInfoIds.ToArray());
    }

    public async Task<IPagedList<NavbarElement>> GetNavbarElementsByNavbarInfoIdAsync(int navbarInfoId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (navbarInfoId <= 0)
            return new PagedList<NavbarElement>(new List<NavbarElement>(), pageIndex, pageSize);

        var query = _navbarElementRepository.Table.Where(nbe => nbe.NavbarInfoId == navbarInfoId);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task InsertNavbarElementAsync(NavbarElement navbarElement)
    {
        ThrowIfNull(navbarElement, nameof(navbarElement));
        await _navbarElementRepository.InsertAsync(navbarElement);
        await _staticCacheManager.RemoveByPrefixAsync(CACHE_KEY);
    }
}
