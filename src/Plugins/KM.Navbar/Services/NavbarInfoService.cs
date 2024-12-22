
using Nop.Core;

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

    public async Task InsertNavbarAsync(NavbarInfo navbar)
    {
        var n = navbar.Name.ToLower();
        var allNavbars = _navbarInfoRepository.GetAll(
            query => query.Where(c => !c.Deleted && c.Name.ToLower() == n),
            cache => new CacheKey(CACHE_KEY, [navbar.Name])
            {
                CacheTime = CACHE_TIME
            });

        if (allNavbars.Any())
            return;

        await _navbarInfoRepository.InsertAsync(navbar);
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

    public async Task UpdateNavbarInfoAsync(NavbarInfo navbar)
    {
        ThrowIfNull(navbar, nameof(navbar));
        await _navbarInfoRepository.UpdateAsync(navbar);
    }

    public async Task DeleteNavbarInfosAsync(IEnumerable<NavbarInfo> navbars)
    {
        ThrowIfNull(navbars, nameof(navbars));
        await _navbarInfoRepository.DeleteAsync(navbars.ToList());
    }

    public async Task<IEnumerable<NavbarInfo>> GetNavbarInfoByIdsAsync(IEnumerable<int> navbarInfoIds)
    {
        return await _navbarInfoRepository.GetByIdsAsync(navbarInfoIds.ToArray());
    }

    public async Task<IPagedList<NavbarElement>> GetNavbarElementsByNavbarInfoIdAsync(int navbarInfoId,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
        if (navbarInfoId == 0)
            return new PagedList<NavbarElement>(new List<NavbarElement>(), pageIndex, pageSize);

        var query = from pc in _productCategoryRepository.Table
                    join p in _productRepository.Table on pc.ProductId equals p.Id
                    where pc.CategoryId == navbarInfoId && !p.Deleted
                    orderby pc.DisplayOrder, pc.Id
                    select pc;

        if (!showHidden)
        {
            var navbarInfosQuery = _navbarInfoRepository.Table.Where(c => c.Published);

            //apply store mapping constraints
            var store = await _storeContext.GetCurrentStoreAsync();
            navbarInfosQuery = await _storeMappingService.ApplyStoreMapping(navbarInfosQuery, store.Id);
            query = query.Where(pc => navbarInfosQuery.Any(c => c.Id == pc.CategoryId));
        }

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }
}
