using Nop.Core.Domain.Directory;
using Nop.Services.Directory;

namespace KM.Catalog.ScheduledTasks;

public partial class UpdateCatalogTask : IScheduleTask
{
    #region fields

    private readonly Dictionary<ProductType, string> _productTypeNames = new();
    private readonly CurrencySettings _currencySettings;
    private readonly ICategoryService _categoryService;
    private readonly ICurrencyService _currencyService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IMediaItemInfoService _mediaItemInfoService;
    private readonly ILanguageService _languageService;
    private readonly IPictureService _pictureService;
    private readonly IProductService _productService;
    private readonly IProductTagService _productTagService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IRepository<KmStoresSnapshot> _storeSnapshotRepository;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly IStructuredDataService _structuredDataService;
    private readonly IVendorService _vendorService;
    private readonly IVideoService _videoService;
    private readonly ILogger _logger;

    private static Queue<DateTime> _updateRequestQueue = new();
    #endregion

    #region ctor
    public UpdateCatalogTask(
        CurrencySettings currencySettings,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        IManufacturerService manufacturerService,
        IMediaItemInfoService mediaItemInfoService,
        ILanguageService languageService,
        IPictureService pictureService,
        IProductService productService,
        IProductTagService productTagService,
        IUrlRecordService urlRecordService,
        IRepository<KmStoresSnapshot> storeSnapshotRepository,
        ISettingService settingService,
        IStoreService storeService,
        IStructuredDataService structuredDataService,
        IVendorService vendorService,
        IVideoService videoService,
        ILogger logger)
    {
        _currencySettings = currencySettings;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _manufacturerService = manufacturerService;
        _mediaItemInfoService = mediaItemInfoService;
        _languageService = languageService;
        _pictureService = pictureService;
        _productService = productService;
        _productTagService = productTagService;
        _urlRecordService = urlRecordService;
        _storeSnapshotRepository = storeSnapshotRepository;
        _settingService = settingService;
        _storeService = storeService;
        _structuredDataService = structuredDataService;
        _vendorService = vendorService;
        _videoService = videoService;
        _logger = logger;
    }

    #endregion

    internal static void EnqueueCatalogUpdateRequest() =>
        _updateRequestQueue.Enqueue(DateTime.UtcNow);

    public async Task ExecuteAsync()
    {
        if (_updateRequestQueue.Count == 0)
            return;

        var cur = DateTime.UtcNow;
        var iteration = TimeSpan.FromMinutes(5);

        //remove all update request from the past 5 minutes
        while (_updateRequestQueue.TryPeek(out var result) && cur - result <= iteration)
            _updateRequestQueue.Dequeue();

        //move to settings
        await _logger.InformationAsync("Start catalog updating process");
        var languageId = _languageService.GetAllLanguages(showHidden: true)
            .Where(l => l.Published)
            .OrderBy(l => l.DisplayOrder).First().Id;

        var storeInfos = await GetStoresSnapshot(languageId);

        var l = await _storeSnapshotRepository.GetAllAsync(q => q.OrderByDescending(x => x.Version).Take(1));
        var last = l?.FirstOrDefault();

        var v = (uint)1;
        if (last != null && last.Version < uint.MaxValue)
            v = last.Version + 1;

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        var data = JsonSerializer.Serialize(storeInfos, options);
        var storeSnapshot = new KmStoresSnapshot
        {
            Data = data,
            Version = v,
        };
        await _storeSnapshotRepository.InsertAsync(storeSnapshot);
    }

    private async Task<IEnumerable<StoreInfo>> GetStoresSnapshot(int languageId)
    {
        var stores = await _storeService.GetAllStoresAsync();
        var mis = await GetManufacturerInfos();
        var vis = await GetVendorInfos();

        var res = new List<StoreInfo>();

        foreach (var store in stores)
        {
            var sis = await _settingService.LoadSettingAsync<StoreInformationSettings>(store.Id);
            if (sis.StoreClosed)
                continue;

            var logoPicture = await _pictureService.GetPictureByIdAsync(sis.LogoPictureId);
            var thumb = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbnail, logoPicture, 0);
            var pic = await ToCatalogMediaInfo(Consts.MediaTypes.Image, logoPicture, 0);

            var sdObj = await _structuredDataService.GenerateStoreStructuredDataAsync(store);
            var sd = Array.Empty<string>();
            if (sdObj != default)
                sd = new[] { JsonSerializer.Serialize(sdObj) };

            var storeProducts = await GetProductsByStoreId(store.Id, languageId, mis, vis);
            var storeVendors = storeProducts
                .Select(p => p.Vendor).DistinctBy(v => v?.Id)
                .ToList();

            res.Add(new StoreInfo
            {
                Id = store.Id.ToString(),
                Name = store.Name,
                LogoThumb = thumb,
                LogoPicture = pic,
                Products = storeProducts,
                StructuredData = sd,
                Vendors = storeVendors,
            });
        }

        return res;
    }

    private async Task<IEnumerable<VendorInfo>> GetVendorInfos()
    {
        var pageIndex = 0;
        const int pageSize = 25;
        IPagedList<Vendor> page;
        var res = new List<VendorInfo>();

        do
        {
            page = await _vendorService.GetAllVendorsAsync(pageIndex: pageIndex, pageSize: pageSize);

            foreach (var v in page)
            {
                if (v.Deleted || !v.Active)
                    continue;

                var picture = await _pictureService.GetPictureByIdAsync(v.PictureId);

                var logo = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbnail, picture, 0);
                var vId = v.Id.ToString();

                res.Add(new VendorInfo
                {
                    Id = vId,
                    Name = v.Name,
                    Logo = logo,
                });
            }
            pageIndex++;
        } while (page.HasNextPage);

        return res;
    }

    private async Task<IEnumerable<ProductInfoDocument>> GetProductsByStoreId(
        int storeId,
        int languageId,
        IEnumerable<(IEnumerable<int> productIds, ManufacturerInfo info)> productManufacturerInfos,
        IEnumerable<VendorInfo> vendorInfos)
    {
        var pageIndex = 0;
        var pageSize = 50;
        IPagedList<Product> page;
        var pis = new List<ProductInfoDocument>();

        do
        {
            page = await _productService.SearchProductsAsync(
                storeId: storeId,
                pageIndex: pageIndex,
                pageSize: pageSize,
                overridePublished: true);

            var products = page
                .Where(p => !p.Deleted &&
                       p.Published &&
                        (p.AvailableStartDateTimeUtc == null && p.AvailableEndDateTimeUtc == null ||
                        p.AvailableStartDateTimeUtc == null && p.AvailableEndDateTimeUtc > DateTime.UtcNow ||
                        p.AvailableStartDateTimeUtc < DateTime.UtcNow && p.AvailableEndDateTimeUtc == null));

            foreach (var p in products)
            {
                var mis = productManufacturerInfos.Where(m => m.productIds.Contains(p.Id)).Select(m => m.info).ToList();

                var tags = await GetProductTags(p.Id);
                var tierPricesSources = await _productService.GetTierPricesByProductAsync(p.Id);
                var tierPrices = tierPricesSources.Select(t => new TierPriceDocument
                {
                    Quantity = t.Quantity,
                    Price = (float)t.Price,
                    StartDateTimeUtc = t.StartDateTimeUtc,
                    EndDateTimeUtc = t.EndDateTimeUtc
                }).ToList();

                var pcs = await _categoryService.GetProductCategoriesByProductIdAsync(p.Id);

                var categories = new List<string>();

                foreach (var pc in pcs)
                {
                    var c = await _categoryService.GetCategoryByIdAsync(pc.CategoryId);
                    var cc = await _categoryService.GetCategoryBreadCrumbAsync(c);
                    var bc = await _categoryService.GetFormattedBreadCrumbAsync(c);
                    categories.Add(bc);
                }

                var slug = await _urlRecordService.GetSeNameAsync(p, languageId: languageId);
                var currency = await getPrimaryCurrency();
                var sdObj = await _structuredDataService.GenerateProductStructuredDataAsync(p, currency);
                var sd = Array.Empty<string>();
                if (sdObj != default)
                    sd = new[] { JsonSerializer.Serialize(sdObj) };

                var pi = new ProductInfoDocument
                {
                    Id = p.Id.ToString(),

                    Categories = categories,
                    DisplayOrder = p.DisplayOrder,
                    Name = p.Name,
                    FullDescription = p.FullDescription,
                    Gtin = p.Gtin,
                    Height = (float)p.Height,
                    IsNew = CheckIsNew(p),
                    IsShipEnabled = p.IsShipEnabled,
                    Length = (float)p.Length,
                    Media = await GetProductMedia(p),
                    Manufacturers = mis,
                    Mpn = p.ManufacturerPartNumber,
                    OldPrice = (float)p.OldPrice,
                    OrderMinimumQuantity = p.OrderMinimumQuantity,
                    ParentGroupedProductId = p.ParentGroupedProductId,
                    Price = (float)p.Price,
                    ProductType = getProductTypeName(p.ProductType),
                    Quantity = p.StockQuantity,
                    Rating = p.ApprovedRatingSum,
                    Reviews = p.ApprovedTotalReviews,
                    ShippingCost = p.IsFreeShipping ? 0 : (float)p.AdditionalShippingCharge,
                    Sku = p.Sku,
                    ShortDescription = p.ShortDescription,
                    StructuredData = sd,
                    Tags = tags,
                    TierPrices = tierPrices,
                    Vendor = vendorInfos.FirstOrDefault(x => x.Id == p.VendorId.ToString()),
                    VisibleIndividually = p.VisibleIndividually,
                    Weight = (float)p.Weight,
                    Width = (float)p.Width,
                    Slug = slug,
                };
                pis.Add(pi);
            }
            pageIndex++;

        } while (page.HasNextPage);

        return pis;

        Task<Currency> getPrimaryCurrency()
        {
            var primaryStoreCurrencyId = _currencySettings.PrimaryStoreCurrencyId;
            return _currencyService.GetCurrencyByIdAsync(primaryStoreCurrencyId);
        }

        string getProductTypeName(ProductType productType)
        {
            if (!_productTypeNames.TryGetValue(productType, out var name))
            {
                name = Enum.GetName(typeof(ProductType), productType);
                _productTypeNames[productType] = name;
            }
            return name;
        }
    }

    private async Task<IEnumerable<(IEnumerable<int> productIds, ManufacturerInfo info)>> GetManufacturerInfos()
    {
        var pageIndex = 0;
        const int pageSize = 25;
        IPagedList<Manufacturer> page;
        var res = new List<(IEnumerable<int> productIds, ManufacturerInfo info)>();

        do
        {
            page = await _manufacturerService.GetAllManufacturersAsync(pageSize: pageSize, pageIndex: pageIndex, overridePublished: true);

            foreach (var m in page)
            {
                if (m.Deleted || !m.Published)
                    continue;
                var manufacturerProductIds = await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(m.Id);
                var productIds = manufacturerProductIds.Select(x => x.ProductId).ToList();

                var picture = await _pictureService.GetPictureByIdAsync(m.PictureId);
                var info = new ManufacturerInfo
                {
                    Name = m.Name,
                    Picture = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbnail, picture, 0),
                };
                res.Add((productIds, info));
            }
            pageIndex++;
        } while (page.HasNextPage);

        return res;
    }


    private async Task<IEnumerable<string>> GetProductTags(int productId)
    {
        var tags = await _productTagService.GetAllProductTagsByProductIdAsync(productId);
        return tags.Select(t => t.Name);
    }

    private async Task<IEnumerable<CatalogMediaInfo>> GetProductMedia(Product product)
    {
        var cmis = new List<CatalogMediaInfo>();

        var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
        var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);

        //create thumb
        var thumb = productPictures.OrderBy(x => x.DisplayOrder).FirstOrDefault();
        if (thumb != default)
        {
            var thumbCmi = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbnail, pictures.First(x => x.Id == thumb.PictureId), thumb.DisplayOrder);
            cmis.Add(thumbCmi);
        }

        //create images
        foreach (var pic in pictures)
        {
            var displayOrder = productPictures.FirstOrDefault(x => x.PictureId == pic.Id)?.DisplayOrder ?? 0;
            var cmi = await ToCatalogMediaInfo(Consts.MediaTypes.Image, pic, displayOrder);
            cmis.Add(cmi);
        }

        var videos = await _videoService.GetVideosByProductIdAsync(product.Id);
        var productVideos = await _productService.GetProductVideosByProductIdAsync(product.Id);
        foreach (var vid in videos)
        {
            var displayOrder = productVideos.FirstOrDefault(x => x.VideoId == vid.Id)?.DisplayOrder ?? 0;
            cmis.Add(ToVideoMediaInfo(vid, displayOrder));
        }

        return cmis;
    }

    private bool CheckIsNew(Product product)
    {
        return product.MarkAsNew &&
            (product.MarkAsNewStartDateTimeUtc == null && product.MarkAsNewEndDateTimeUtc == null ||
            product.MarkAsNewStartDateTimeUtc == null && product.MarkAsNewEndDateTimeUtc > DateTime.UtcNow ||
            product.MarkAsNewStartDateTimeUtc < DateTime.UtcNow && product.MarkAsNewEndDateTimeUtc == null);
    }


    private async Task<CatalogMediaInfo> ToCatalogMediaInfo(string type, Picture picture, int displayOrder)
    {
        if (picture == default)
            return default;

        var mii = await _mediaItemInfoService.GetOrCreateMediaItemInfoAsync(type, picture, displayOrder);
        return new CatalogMediaInfo
        {
            Alt = picture.AltAttribute,
            DisplayOrder = displayOrder,
            Title = picture.TitleAttribute,
            Type = mii.Type,
            Uri = mii.Uri
        };
    }
    private CatalogMediaInfo ToVideoMediaInfo(Video video, int displayOrder = 0)
    {
        return new CatalogMediaInfo
        {
            Type = "video",
            Uri = video.VideoUrl,
            DisplayOrder = displayOrder
        };
    }
}
