using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Services.Media;
using Nop.Core.Domain.Media;
using SixLabors.ImageSharp;
using Nop.Plugin.Misc.KM.Catalog.Services;
using System.Text.Json;
using System.IO;
using Nop.Core.Domain.Stores;
using Microsoft.IdentityModel.Tokens;
using Nop.Plugin.Misc.KM.Catalog.Documents;
using Microsoft.AspNetCore.SignalR;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.KM.Catalog
{
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
        private readonly IStorageManager _storageManager;
        private readonly IStore<ProductInfoDocument> _store;
        private readonly IHubContext<CatalogHub> _hub;
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
            IStorageManager storageManager,
            IStore<ProductInfoDocument> store,
            ICategoryService categoryService,
            IHubContext<CatalogHub> hub,
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
            _storageManager = storageManager;
            _store = store;
            _categoryService = categoryService;
            _hub = hub;
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

            var catalogProducts = new List<ProductInfoDocument>();

            var stores = await _storeService.GetAllStoresAsync();
            var vendors = await _vendorService.GetAllVendorsAsync();
            var mis = await GetManufacturerInfos();

            var sis = new List<StoreInfo>();
            foreach (var store in stores)
            {
                var storeVendors = new List<VendorInfo>();
                sis.Add(new StoreInfo
                {
                    id = store.Id.ToString(),
                    name = store.Name,
                    vendors = storeVendors,
                });
                var products = await _productService.SearchProductsAsync(storeId: store.Id, overridePublished: true);
                var productsByVendor = products
                    .Where(CheckIsAvailable)
                    .GroupBy(x => x.VendorId);

                foreach (var pv in productsByVendor)
                {
                    //no vendor
                    if (pv.Key == 0)
                        continue;

                    var vendor = vendors.First(x => x.Id == pv.Key);
                    var vi = await ToVendorInfos(vendor, store, pv, mis);

                    var path = $"catalog/{store.Name}/{vendor.Name}.json";
                    await UploadAsJsonAsync(path, vi);
                    storeVendors.Add(vi with { products = null });
                    catalogProducts.AddRange(vi.products);
                }
            }

            sis = sis.Where(s => !s.vendors.IsNullOrEmpty()).ToList();
            try
            {
                await UploadAsJsonAsync($"catalog/stores.json", new { stores = sis });

                await _store.CreateOrUpdateAsync(catalogProducts);
            }
            catch (Exception ex)
            {
                EnqueueCatalogUpdateRequest();
                throw ex;
            }
            await _hub.Clients.All.SendAsync("updated");
        }

        private async Task UploadAsJsonAsync<TData>(string path, TData data)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };

            var bytes = JsonSerializer.SerializeToUtf8Bytes(data, options);
            using var stream = new MemoryStream(bytes);
            {
                _ = await _storageManager.UploadAsync(path, "application/json", stream);
            }
        }

        private async Task<IEnumerable<(IEnumerable<int> productIds, ManufacturerInfo info)>> GetManufacturerInfos()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(overridePublished: true);

            var res = new List<(IEnumerable<int> productIds, ManufacturerInfo info)>();
            foreach (var m in manufacturers)
            {
                if (m.Deleted || !m.Published)
                    continue;
                var manufacturerProductIds = await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(m.Id);
                var productIds = manufacturerProductIds.Select(x => x.ProductId).ToList();

                var picture = await _pictureService.GetPictureByIdAsync(m.PictureId);
                var info = new ManufacturerInfo
                {
                    name = m.Name,
                    picture = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbs, picture, 0),
                };
                res.Add((productIds, info));
            }
            return res;
        }

        private async Task<VendorInfo> ToVendorInfos(
            Vendor vendor,
            Store store,
            IEnumerable<Product> vendorProducts,
            IEnumerable<(IEnumerable<int> productIds, ManufacturerInfo info)> manufacturersInfos)
        {
            var pis = new List<ProductInfoDocument>();
            foreach (var p in vendorProducts)
            {
                var mis = manufacturersInfos.Where(m => m.productIds.Contains(p.Id)).Select(m => m.info).ToList();
                var tags = await GetProductTags(p.Id);
                var tierPricesSources = await _productService.GetTierPricesByProductAsync(p.Id);
                var tierPrices = tierPricesSources.Select(t => new TierPriceDocument
                {
                    quantity = t.Quantity,
                    price = (float)t.Price,
                    startDateTimeUtc = t.StartDateTimeUtc,
                    endDateTimeUtc = t.EndDateTimeUtc
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
                    id = p.Id.ToString(),
                    name = p.Name,
                    productType = GetProductTypeName(p.ProductType),
                    parentGroupedProductId = p.ParentGroupedProductId,
                    visibleIndividually = p.VisibleIndividually,
                    shortDescription = p.ShortDescription,
                    fullDescription = p.FullDescription,
                    rating = p.ApprovedRatingSum,
                    reviews = p.ApprovedTotalReviews,
                    sku = p.Sku,
                    mpn = p.ManufacturerPartNumber,
                    gtin = p.Gtin,
                    isShipEnabled = p.IsShipEnabled,
                    shippingCost = p.IsFreeShipping ? 0 : (float)p.AdditionalShippingCharge,
                    quantity = p.StockQuantity,
                    orderMinimumQuantity = p.OrderMinimumQuantity,
                    price = (float)p.Price,
                    tierPrices = tierPrices,
                    oldPrice = (float)p.OldPrice,
                    isNew = CheckIsNew(p),
                    weight = (float)p.Weight,
                    length = (float)p.Length,
                    width = (float)p.Width,
                    height = (float)p.Height,
                    displayOrder = p.DisplayOrder,
                    media = await GetProductMedia(p),
                    manufacturers = mis,
                    tags = tags,
                    categories = categories
                };
                pis.Add(pi);
            }

            CatalogMediaInfo logo = null;
            if (vendor.PictureId != 0)
            {
                var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
                logo = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbs, picture, 0);
            }
            return new VendorInfo
            {
                id = vendor.Id.ToString(),
                name = vendor.Name,
                logo = logo,
                store = new()
                {
                    id = store.Id.ToString(),
                    name = store.Name,
                },
                products = pis,
            };
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
                var thumbCmi = await ToCatalogMediaInfo(Consts.MediaTypes.Thumbs, pictures.First(x => x.Id == thumb.PictureId), thumb.DisplayOrder);
                cmis.Add(thumbCmi);
            }

            //create images
            foreach (var pic in pictures)
            {
                var displayOrder = productPictures.FirstOrDefault(x => x.PictureId == pic.Id)?.DisplayOrder ?? 0;
                var cmi = await ToCatalogMediaInfo(Consts.MediaTypes.Images, pic, displayOrder);
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

        private bool CheckIsAvailable(Product product)
        {
            return
                !product.Deleted &&
                product.Published &&
                ((product.AvailableStartDateTimeUtc == null && product.AvailableEndDateTimeUtc == null) ||
                (product.AvailableStartDateTimeUtc == null && product.AvailableEndDateTimeUtc > DateTime.UtcNow) ||
                (product.AvailableStartDateTimeUtc < DateTime.UtcNow && product.AvailableEndDateTimeUtc == null));
        }

        private bool CheckIsNew(Product product)
        {
            return product.MarkAsNew &&
                ((product.MarkAsNewStartDateTimeUtc == null && product.MarkAsNewEndDateTimeUtc == null) ||
                (product.MarkAsNewStartDateTimeUtc == null && product.MarkAsNewEndDateTimeUtc > DateTime.UtcNow) ||
                (product.MarkAsNewStartDateTimeUtc < DateTime.UtcNow && product.MarkAsNewEndDateTimeUtc == null));
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
            var mii = await _mediaItemInfoService.GetOrCreateMediaItemInfoAsync(type, picture, displayOrder);

            return new CatalogMediaInfo
            {
                alt = picture.AltAttribute,
                displayOrder = displayOrder,
                title = picture.TitleAttribute,
                type = mii.Type,
                uri = mii.Uri
            };
        }
        private CatalogMediaInfo ToVideoMediaInfo(Video video, int displayOrder = 0)
        {
            return new CatalogMediaInfo
            {
                type = "video",
                uri = video.VideoUrl,
                displayOrder = displayOrder
            };
        }
    }
}
