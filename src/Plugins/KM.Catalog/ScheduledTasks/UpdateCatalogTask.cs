namespace KM.Catalog.ScheduledTasks;

public partial class UpdateCatalogTask : IScheduleTask
{
    #region fields

    private readonly IStoreService _storeService;
    private readonly IVendorService _vendorService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IProductTagService _productTagService;
    private readonly IPictureService _pictureService;
    private readonly IVideoService _videoService;
    private readonly IMediaItemInfoService _mediaItemInfoService;
    private readonly ISettingService _settingService;
    private readonly IRepository<KmStoresSnapshot> _storeSnapshotRepository;
    private readonly ILogger _logger;

    private readonly Dictionary<ProductType, string> _productTypeNames;
    private static Queue<DateTime> _updateRequestQueue = new();

    #endregion

    #region ctor
    public UpdateCatalogTask(
        IStoreService storeService,
        IVendorService vendorService,
        IManufacturerService manufacturerService,
        IProductService productService,
        IProductTagService productTagService,
        IPictureService pictureService,
        IVideoService videoService,
        IMediaItemInfoService mediaItemInfoService,
        ICategoryService categoryService,
        IRepository<KmStoresSnapshot> storeSnapshotRepository,
        ISettingService settingService,
        ILogger logger)
    {
        _storeService = storeService;
        _vendorService = vendorService;
        _manufacturerService = manufacturerService;
        _productService = productService;
        _productTagService = productTagService;
        _pictureService = pictureService;
        _videoService = videoService;

        _productTypeNames = new Dictionary<ProductType, string>();

        _mediaItemInfoService = mediaItemInfoService;
        _categoryService = categoryService;
        _storeSnapshotRepository = storeSnapshotRepository;
        _settingService = settingService;
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

        await _logger.InformationAsync("Start catalog updating process");
        var storeInfos = await GetStoresSnapshot();

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

        var json = JsonSerializer.Serialize(storeInfos, options);
        var storeSnapshot = new KmStoresSnapshot
        {
            Json = json,
            Version = v,
        };
        await _storeSnapshotRepository.InsertAsync(storeSnapshot);
    }

    private async Task<IEnumerable<StoreInfo>> GetStoresSnapshot()
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

            var storeProducts = await GetProductsByStoreId(store.Id, mis, vis);
            var storeVendors = storeProducts
                .Select(p => p.Vendor).DistinctBy(v => v.Id)
                .ToList();

            res.Add(new StoreInfo
            {
                Id = store.Id.ToString(),
                Name = store.Name,
                LogoThumb = thumb,
                LogoPicture = pic,
                Products = storeProducts,
                Vendors = storeVendors
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
                    ProductType = GetProductTypeName(p.ProductType),
                    Quantity = p.StockQuantity,
                    Rating = p.ApprovedRatingSum,
                    Reviews = p.ApprovedTotalReviews,
                    ShippingCost = p.IsFreeShipping ? 0 : (float)p.AdditionalShippingCharge,
                    Sku = p.Sku,
                    ShortDescription = p.ShortDescription,
                    Tags = tags,
                    TierPrices = tierPrices,
                    Vendor = vendorInfos.FirstOrDefault(x => x.Id == p.VendorId.ToString()),
                    VisibleIndividually = p.VisibleIndividually,
                    Weight = (float)p.Weight,
                    Width = (float)p.Width,
                };
                pis.Add(pi);
            }
            pageIndex++;

        } while (page.HasNextPage);

        return pis;
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

        foreach (var pic in pictures)
        {
            var displayOrder = productPictures.FirstOrDefault(x => x.PictureId == pic.Id)?.DisplayOrder ?? 0;
            var thumbCmi = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbnail, pic, displayOrder);
            var cmi = await ToCatalogMediaInfo(Consts.MediaTypes.Image, pic, displayOrder);

            cmis.AddRange(new[] { thumbCmi, cmi });
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

    private string GetProductTypeName(ProductType productType)
    {
        if (!_productTypeNames.TryGetValue(productType, out var name))
        {
            name = Enum.GetName(typeof(ProductType), productType);
            _productTypeNames[productType] = name;
        }
        return name;
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
