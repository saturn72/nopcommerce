
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace KM.Api.Controllers;

[Route("api/search")]
public class SearchController : KmApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly ILanguageService _languageService;
    private readonly ICurrencyService _currencyService;
    private readonly CurrencySettings _currencySettings;
    private readonly IPictureService _pictureService;
    private readonly IVideoService _videoService;

    public SearchController(
        IProductService productService,
        IUrlRecordService urlRecordService,
        ILanguageService languageService,
        ICurrencyService currencyService,
        CurrencySettings currencySettings,
        IPictureService pictureService,
        IVideoService videoService)
    {
        _productService = productService;
        _urlRecordService = urlRecordService;
        _languageService = languageService;
        _currencyService = currencyService;
        _pictureService = pictureService;
        _videoService = videoService;
        _currencySettings = currencySettings;
    }

    [HttpGet]
    public async Task<IActionResult> Query(
        [FromQuery(Name = "q")] string? keywords,
        [FromQuery] int storeId,
        [FromQuery] int offset = 0,
        [FromQuery] int pageSize = 50)
    {
        var products = await _productService.SearchProductsAsync(
            offset,
            pageSize,
            storeId: storeId,
            keywords: keywords,
            searchDescriptions: true,
            searchManufacturerPartNumber: true,
            searchProductTags: true,
            searchSku: true
            );

        var epoch = DateTimeOffset.Now.ToUnixTimeSeconds();

        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var currencyName = currency.CurrencyCode;

        var languageId = _languageService.GetAllLanguages(showHidden: true)
            .Where(l => l.Published)
            .OrderBy(l => l.DisplayOrder).First().Id;


        var m = new List<object>();
        foreach (var p in products)
        {
            var slug = await _urlRecordService.GetSeNameAsync(p);

            var gallery = await GetProductGalleryAsync(p);
            var reviews = await GetProductReviewsAsync(p);
            var i = new
            {
                id = p.Id,
                banner = getBanner(p, epoch),
                name = p.Name,
                productPrice = p.Price,
                currentPrice = p.OldPrice > 0 ? p.OldPrice : p.Price,
                fullDescription = p.FullDescription,
                shortDescription = p.ShortDescription,
                showStockQuantity = p.DisplayStockQuantity,
                stockQuantity = p.DisplayStockQuantity ? p.StockQuantity : 0,
                sku = p.Sku,
                gtin = p.Gtin,
                mpn = p.ManufacturerPartNumber,
                slug = slug,
                currency = currencyName,
                //attributes = productAttributes,
                gallery = gallery,
                reviews = reviews,
                //unit =
            };

            m.Add(i);
        }

        return ToJsonResult(m);

        static string getBanner(Product product, long epoch)
        {
            if (product.MarkAsNew &&
                    (product.MarkAsNewEndDateTimeUtc == null ||
                    (epoch - new DateTimeOffset(product.MarkAsNewEndDateTimeUtc.Value).ToUnixTimeSeconds() > 0)))
                return "new";

            if (product.HasDiscountsApplied)
                return "sale";
            return "none";
        }
    }

    private async Task<IEnumerable<object>> GetProductReviewsAsync(Product p)
    {
        var reviews = await _productService.GetAllProductReviewsAsync(productId: p.Id, approved: true);
        return reviews.Select(r => new
        {
            title = r.Title,
            reviewText = r.ReviewText,
            replyText = r.ReplyText,
            rating = r.Rating,
            helpfulYesTotal = r.HelpfulYesTotal,
            helpfulNoTotal = r.HelpfulNoTotal,
            createdOnUtc = r.CreatedOnUtc,
        }).ToList();

    }

    private async Task<IEnumerable<object>> GetProductGalleryAsync(Product product)
    {
        var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
        var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);

        var cmis = new List<object>();

        foreach (var pp in productPictures)
        {
            var picture = pictures.FirstOrDefault(p => p.Id == pp.PictureId);
            if (picture == null)
                continue;

            var (url, _) = await _pictureService.GetPictureUrlAsync(picture);

            var item = new
            {
                alt = picture.AltAttribute,
                type = "image",
                index = pp.DisplayOrder,
                url = url,
                mimeType = picture.MimeType,
#warning - optimize for thumb
                thumb = picture.VirtualPath,
                seoFilename = picture.SeoFilename,
                title = picture.TitleAttribute,
            };
            cmis.Add(item);
        }

        var videos = await _videoService.GetVideosByProductIdAsync(product.Id);
        var productVideos = await _productService.GetProductVideosByProductIdAsync(product.Id);
        foreach (var v in productVideos)
        {
            var video = videos.FirstOrDefault(p => p.Id == v.VideoId);
            if (video == null)
                continue;

            var item = new
            {
                type = "video",
                index = v.DisplayOrder,
                url = video.VideoUrl,
            };
            cmis.Add(item);
        }
        return cmis.ToList();
    }
}
