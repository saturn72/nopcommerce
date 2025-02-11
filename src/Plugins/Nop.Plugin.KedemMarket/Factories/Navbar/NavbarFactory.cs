using KedemMarket.Common;
using KedemMarket.Models.Catalog;
using KedemMarket.Factories.Catalog;
using KedemMarket.Models.Media;
using KedemMarket.Models.Navbar;
using KedemMarket.Services.Media;
using KedemMarket.Services.Navbar;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Media;
using Nop.Services.Vendors;

namespace KedemMarket.Factories.Navbar;

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
    private readonly IProductService _productService;
    private readonly IProductApiFactory _productApiFactory;

    public NavbarFactory(
        INavbarService navbarService,
        IVendorService vendorService,
        MediaConvertor mediaConverter,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        IGenericAttributeService genericAttributeService,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IProductService productService,
        IProductApiFactory productApiFactory)
    {
        _navbarService = navbarService;
        _vendorService = vendorService;
        _mediaConverter = mediaConverter;
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
        _vendorAttributeService = vendorAttributeService;
        _genericAttributeService = genericAttributeService;
        _vendorAttributeParser = vendorAttributeParser;
        _productService = productService;
        _productApiFactory = productApiFactory;
    }

    public async Task<NavbarAppModel> PrepareNavbarApiModelByNameAsync(string name)
    {
        var key = new CacheKey($"{NavbarCacheSettings.NAVBAR_CACHE_KEY}.{name}", NavbarCacheSettings.NAVBAR_CACHE_KEY)
        {
            CacheTime = NavbarCacheSettings.CACHE_TIME
        };

        var nam = await _staticCacheManager.GetAsync<NavbarAppModel>(key);
        if (nam != null)
            return nam;

        var navbar = await _navbarService.GetNavbarInfoByNameAsync(name);
        if (navbar == null)
            return null;

        var elms = navbar?.Elements ?? [];
        var elements = new List<NavbarElementModel>();

        var allVendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
        var shortDescriptionAttribute = allVendorAttributes.First(va => va.Name == KmConsts.VendorAttributeNames.ShortDescription);

        foreach (var e in elms)
        {
            var nevs = await _navbarService.GetNavbarElementVendorsByNavbarElementIdAsync(e.Id);
            var vendors = await nevs.Where(x => x.Published)
                .SelectAwait(async nev => await _vendorService.GetVendorByIdAsync(nev.VendorId))
                .ToListAsync();

            var gitTemp = new Dictionary<int, Task<GalleryItemModel>>();
            var vpTemp = new Dictionary<int, Task<IPagedList<Product>>>();
            foreach (var v in vendors)
            {
                var gitTask = async () =>
                {
                    if (v.PictureId <= 0)
                        return null;

                    var pic = await _pictureService.GetPictureByIdAsync(v.PictureId);
                    return await _mediaConverter.ToGalleryItemModel(pic, 0);
                };
                gitTemp[v.Id] = gitTask();
                vpTemp[v.Id] = _productService.SearchProductsAsync(vendorId: v.Id);
            }
            await Task.WhenAll(gitTemp.Values);
            await Task.WhenAll(vpTemp.Values);

            var vendorModels = new List<VendorModel>();
            foreach (var v in vendors)
            {
                var selectedVendorAttributes = await _genericAttributeService.GetAttributeAsync<string>(v, NopVendorDefaults.VendorAttributes);
                var shortDescription = _vendorAttributeParser.ParseValues(selectedVendorAttributes, shortDescriptionAttribute.Id).FirstOrDefault();
                _ = gitTemp.TryGetValue(v.Id, out var pic);


                var products = await _productApiFactory.ToProductInfoApiModelAsync(await vpTemp[v.Id]);

                var productSlims = products.Select(p => new ProductSlimApiModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Banners = p.Banners,
                    Gallery = [p.Gallery.FirstOrDefault()],
                    Price = p.Price,
                    PriceText = p.PriceText,
                    PriceOld = p.PriceOld,
                    PriceOldText = p.PriceOldText,
                    PriceWithDiscount = p.PriceWithDiscount,
                    PriceWithDiscountText = p.PriceWithDiscountText,
                    Variants = p.Variants,
                    Slug = p.Slug,
                });

                vendorModels.Add(new()
                {
                    Id = v.Id,
                    Name = v.Name,
                    Picture = pic?.Result,
                    ShortDescription = shortDescription,
                    Products = productSlims
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