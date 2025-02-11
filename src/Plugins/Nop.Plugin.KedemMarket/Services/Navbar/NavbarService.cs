namespace KedemMarket.Services.Navbar;

using KedemMarket.Admin.Domain;

public class NavbarService : INavbarService
{
    private readonly IRepository<NavbarInfo> _navbarInfoRepository;
    private readonly IRepository<NavbarElement> _navbarElementRepository;
    private readonly IRepository<NavbarElementVendor> _navbarElementVendorRepository;
    private readonly IStoreMappingService _storeMappingService;

    public NavbarService(
        IRepository<NavbarInfo> navbarInfoRepository,
        IRepository<NavbarElement> navbarElementRepository,
        IRepository<NavbarElementVendor> navbarElementVendorRepository,
        IStoreMappingService storeMappingService)
    {
        _navbarInfoRepository = navbarInfoRepository;
        _navbarElementRepository = navbarElementRepository;
        _storeMappingService = storeMappingService;
        _navbarElementVendorRepository = navbarElementVendorRepository;
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
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

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
            query => query.Where(c => !c.Deleted && c.Name.ToLower() == n));

        if (allNavbars.Any())
            return;

        await _navbarInfoRepository.InsertAsync(navbarInfo);
    }

    public async Task<NavbarInfo> GetNavbarInfoByIdAsync(int id)
    {
        return await _navbarInfoRepository.GetByIdAsync(id);
    }

    public async Task<NavbarInfo> GetNavbarInfoByNameAsync(string name)
    {
        var navbar = await _navbarInfoRepository.Table.FirstOrDefaultAsync(c => c.Name == name);

        if (navbar != null)
        {
            var elements = await _navbarElementRepository.GetAllAsync(query => query = query.Where(c => c.NavbarInfoId == navbar.Id));
            navbar.Elements = elements.ToList();
        }
        return navbar;

    }

    public async Task UpdateNavbarInfoAsync(NavbarInfo navbarInfo)
    {
        ThrowIfNull(navbarInfo, nameof(navbarInfo));
        await _navbarInfoRepository.UpdateAsync(navbarInfo);
    }

    public async Task DeleteNavbarInfosAsync(IEnumerable<NavbarInfo> navbarInfoss)
    {
        ThrowIfNull(navbarInfoss, nameof(navbarInfoss));
        if (!navbarInfoss.Any())
            return;
        await _navbarInfoRepository.DeleteAsync(navbarInfoss.ToList());
    }

    public async Task<IEnumerable<NavbarInfo>> GetNavbarInfoByIdsAsync(IEnumerable<int> navbarInfoIds)
    {
        return await _navbarInfoRepository.GetByIdsAsync(navbarInfoIds.ToArray());
    }

    #region navbar-elements
    public async Task<IPagedList<NavbarElement>> GetNavbarElementsByNavbarInfoIdAsync(int navbarInfoId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (navbarInfoId <= 0)
            return new PagedList<NavbarElement>(new List<NavbarElement>(), pageIndex, pageSize);

        var query = _navbarElementRepository.Table.Where(nbe => nbe.NavbarInfoId == navbarInfoId);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<NavbarElement> GetNavbarElementsByIdAsync(int navbarElementId)
    {
        if (navbarElementId <= 0)
            throw new ArgumentException($"{nameof(navbarElementId)} must be positive");

        return await _navbarElementRepository.GetByIdAsync(navbarElementId);
    }

    public async Task InsertNavbarElementAsync(NavbarElement navbarElement)
    {
        ThrowIfNull(navbarElement, nameof(navbarElement));
        await _navbarElementRepository.InsertAsync(navbarElement);
    }

    public async Task UpdateNavbarElementAsync(NavbarElement navbarElement)
    {
        ThrowIfNull(navbarElement, nameof(navbarElement));
        await _navbarElementRepository.UpdateAsync(navbarElement);
    }

    public async Task DeleteNavbarElementAsync(NavbarElement navbarElement)
    {
        ThrowIfNull(navbarElement, nameof(navbarElement));
        var nevs = await GetNavbarElementVendorsByNavbarElementIdAsync(navbarElement.Id);

        if (nevs.Any())
            await _navbarElementVendorRepository.DeleteAsync(nevs);

        await _navbarElementRepository.DeleteAsync(navbarElement);
    }

    public async Task<IPagedList<NavbarElementVendor>> GetNavbarElementVendorsByNavbarElementIdAsync(int navbarElementId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (navbarElementId <= 0)
            return new PagedList<NavbarElementVendor>(new List<NavbarElementVendor>(), pageIndex, pageSize);

        var query = _navbarElementVendorRepository.Table.Where(nbe => nbe.NavbarElementId == navbarElementId);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task AddNavbarElementVendorsAsync(int navbarElementId, IList<int> selectedVendorIds)
    {
        var all = selectedVendorIds.Select(svi => new NavbarElementVendor
        {
            VendorId = svi,
            NavbarElementId = navbarElementId,
        }).ToList();
        var exist = (await GetNavbarElementVendorsByNavbarElementIdAsync(navbarElementId)).ToList();

        var toAdd = all.Where(a => exist.All(e => e.VendorId != a.VendorId)).ToList();
        if (toAdd.Any())
            await _navbarElementVendorRepository.InsertAsync(toAdd);
    }

    public async Task<NavbarElementVendor> GetNavbarElementVendorByIdAsync(int id)
    {
        return await _navbarElementVendorRepository.GetByIdAsync(id);
    }

    public async Task DeleteNavbarElementVendorAsync(NavbarElementVendor navbarElementVendor)
    {
        await _navbarElementVendorRepository.DeleteAsync(navbarElementVendor);
    }

    public async Task UpdateNavbarElementVendorAsync(NavbarElementVendor navbarElementVendor)
    {
        ThrowIfNull(navbarElementVendor, nameof(navbarElementVendor));

        await _navbarElementVendorRepository.UpdateAsync(navbarElementVendor);
    }

    public async Task<IPagedList<NavbarElementVendor>> GetNavbarElementVendorsByVendorIdAsync(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (vendorId <= 0)
            return new PagedList<NavbarElementVendor>(new List<NavbarElementVendor>(), pageIndex, pageSize);

        var query = _navbarElementVendorRepository.Table.Where(nbe => nbe.VendorId == vendorId);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion
}
