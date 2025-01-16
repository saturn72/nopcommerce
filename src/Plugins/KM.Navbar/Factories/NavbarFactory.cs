using KM.Common.Models.Media;
using KM.Common.Services.Media;
using KM.Navbar.Admin.Domain;
using KM.Navbar.Models;
using Nop.Services.Media;
using Nop.Services.Vendors;
using static KM.Navbar.Services.NavbarCacheSettings;

namespace KM.Navbar.Factories;

public class NavbarFactory : INavbarFactory
{
    private readonly INavbarService _navbarService;
    private readonly IVendorService _vendorService;
    private readonly MediaConvertor _mediaConverter;
    private readonly IPictureService _pictureService;
    private readonly IStaticCacheManager _staticCacheManager;

    public NavbarFactory(
        INavbarService navbarService,
        IVendorService vendorService,
        MediaConvertor mediaConverter,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager)
    {
        _navbarService = navbarService;
        _vendorService = vendorService;
        _mediaConverter = mediaConverter;
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<NavbarAppModel?> PrepareNavbarApiModelByNameAsync(string name)
    {
        var key = new CacheKey($"{NAVBAR_ELEMENET_CACHE_KEY}.{name}", [NAVBAR_CACHE_KEY, NAVBAR_ELEMENET_CACHE_KEY])
        {
            CacheTime = CACHE_TIME
        };
        NavbarInfo navbar = null;
        var nam = await _staticCacheManager.GetAsync<NavbarAppModel>(key);
        if (nam == null)
            navbar = await _navbarService.GetNavbarInfoByNameAsync(name);

        if (navbar == null)
            return null;

        var elms = navbar?.Elements ?? [];
        var elements = new List<NavbarElementModel>();

        foreach (var e in elms)
        {
            var nevs = await _navbarService.GetNavbarElementVendorsByNavbarElementIdAsync(e.Id);
            var vendors = await nevs.Where(x => x.Published)
                .SelectAwait(async nev => await _vendorService.GetVendorByIdAsync(nev.VendorId))
                .ToListAsync();

            var temp = new Dictionary<int, Task<GalleryItemModel>>();
            foreach (var v in vendors.Where(v => v.PictureId > 0))
            {
                var pic = await _pictureService.GetPictureByIdAsync(v.PictureId);
                var git = _mediaConverter.ToGalleryItemModel(pic, 0);
                temp[v.Id] = git;
            }

            await Task.WhenAll(temp.Values);

            var vendorModels = vendors.Select(v =>
            {
                _ = temp.TryGetValue(v.Id, out var pic);
                return new VendorModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Picture = pic?.Result
                };
            }).ToList();

            //var vendors 
            var ne = new NavbarElementModel
            {
                ActiveIcon = e.ActiveIcon,
                Alt = e.Alt,
                Caption = e.Caption,
                Icon = e.Icon,
                Index = e.Index,
                Tags = e.Tags,
                Type = e.Type,
                Value = e.Value,
                Vendors = vendorModels,
            };
            elements.Add(ne);
        }

        nam = new NavbarAppModel
        {
            Elements = elements,
        };
        await _staticCacheManager.SetAsync<NavbarAppModel>(key, nam);

        return nam;
    }
}