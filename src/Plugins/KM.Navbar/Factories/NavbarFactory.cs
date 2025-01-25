using KM.Common;
using KM.Common.Models.Media;
using KM.Common.Services.Media;
using KM.Navbar.Models;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Common;
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
    private readonly IGenericAttributeService _genericAttributeService;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    private IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;

    public NavbarFactory(
        INavbarService navbarService,
        IVendorService vendorService,
    MediaConvertor mediaConverter,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        IGenericAttributeService genericAttributeService,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser)
    {
        _navbarService = navbarService;
        _vendorService = vendorService;
        _mediaConverter = mediaConverter;
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
        _vendorAttributeService = vendorAttributeService;
        _genericAttributeService = genericAttributeService;
        _vendorAttributeParser = vendorAttributeParser;
    }

    public async Task<NavbarAppModel?> PrepareNavbarApiModelByNameAsync(string name)
    {
        var key = new CacheKey($"{NAVBAR_CACHE_KEY}.{name}", NAVBAR_CACHE_KEY)
        {
            CacheTime = CACHE_TIME
        };

        var nam = await _staticCacheManager.GetAsync<NavbarAppModel>(key);
        if (nam != null)
            return nam;

        var navbar = await _navbarService.GetNavbarInfoByNameAsync(name);
        if (navbar == null)
            return null;

        var elms = navbar?.Elements ?? [];
        var elements = new List<NavbarElementModel>();

        var allVendorAttributes = (await _vendorAttributeService.GetAllAttributesAsync());
        var shortDescriptionAttribute = allVendorAttributes.First(va => va.Name == KmConsts.VendorAttributeNames.ShortDescription);

        foreach (var e in elms)
        {
            var nevs = await _navbarService.GetNavbarElementVendorsByNavbarElementIdAsync(e.Id);
            var vendors = await nevs.Where(x => x.Published)
                .SelectAwait(async nev => await _vendorService.GetVendorByIdAsync(nev.VendorId))
                .ToListAsync();

            var gitTemp = new Dictionary<int, Task<GalleryItemModel>>();
            foreach (var v in vendors.Where(v => v.PictureId > 0))
            {
                var pic = await _pictureService.GetPictureByIdAsync(v.PictureId);
                var git = _mediaConverter.ToGalleryItemModel(pic, 0);
                gitTemp[v.Id] = git;
            }
            await Task.WhenAll(gitTemp.Values);



            //var va = await _vendorAttributeService.GetAttribu();
            //va.First().
            var vendorModels = new List<VendorModel>();

            foreach (var v in vendors)
            {
                var selectedVendorAttributes = await _genericAttributeService.GetAttributeAsync<string>(v, NopVendorDefaults.VendorAttributes);
                var shortDescription = _vendorAttributeParser.ParseValues(selectedVendorAttributes, shortDescriptionAttribute.Id).FirstOrDefault();
                _ = gitTemp.TryGetValue(v.Id, out var pic);

                vendorModels.Add(new()
                {
                    Id = v.Id,
                    Name = v.Name,
                    Picture = pic?.Result,
                    ShortDescription = shortDescription,
                });
            }

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
        await _staticCacheManager.SetAsync(key, nam);

        return nam;
    }
}