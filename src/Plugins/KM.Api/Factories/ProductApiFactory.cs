using DocumentFormat.OpenXml.Presentation;
using KM.Api.Models.Catalog;
using KM.Api.Services.Media;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace KM.Api.Factories;

public class ProductApiFactory : IProductApiFactory
{
    private readonly IProductService _productService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly ILocalizationService _localizationService;
    private readonly ICurrencyService _currencyService;
    private readonly CurrencySettings _currencySettings;
    private readonly IPictureService _pictureService;
    private readonly IVideoService _videoService;
    private readonly IStoreContext _storeContext;
    private readonly MediaSettings _mediaSettings;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly MediaPreperar _mediaPreperar;

    public ProductApiFactory(
        IProductService productService,
        IProductAttributeService productAttributeService,
        IUrlRecordService urlRecordService,
        ILocalizationService localizationService,
        ICurrencyService currencyService,
        CurrencySettings currencySettings,
        IPictureService pictureService,
        IVideoService videoService,
        IStoreContext storeContext,
        MediaSettings mediaSettings,
        IProductAttributeParser productAttributeParser,
        MediaPreperar mediaPreperar)
    {
        _productService = productService;
        _productAttributeService = productAttributeService;
        _urlRecordService = urlRecordService;
        _localizationService = localizationService;
        _currencyService = currencyService;
        _currencySettings = currencySettings;
        _pictureService = pictureService;
        _videoService = videoService;
        _storeContext = storeContext;
        _mediaSettings = mediaSettings;
        _productAttributeParser = productAttributeParser;
        _mediaPreperar = mediaPreperar;
    }

    public async Task<IEnumerable<ProductInfoApiModel>> ToProductApiModel(IEnumerable<Product> products)
    {
        var epoch = DateTimeOffset.Now.ToUnixTimeSeconds();

        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var currencyName = currency.CurrencyCode;
        var store = await _storeContext.GetCurrentStoreAsync();

        var languageId = store.DefaultLanguageId;

        var m = new List<ProductInfoApiModel>();
        foreach (var p in products)
            m.Add(await ToProductInfo(p, languageId, epoch, currencyName));
        return m;
    }
    private async Task<ProductInfoApiModel> ToProductInfo(Product product, int languageId, long epoch, string currencyName)
    {
        var banner = await GetBannerAsync(product, epoch);
        var gallery = await GetProductGalleryAsync(product);
        var slug = await _urlRecordService.GetSeNameAsync(product, languageId);
        var reviews = await GetProductReviewsAsync(product);
        var variants = await GetProductVariantsAsync(product, languageId);
        return new()
        {
            Id = product.Id,
            Banner = banner,
            Currency = currencyName,
            DisplayIndex = product.DisplayOrder,
            Name = product.Name,
            CurrentPrice = product.OldPrice > 0 ? product.OldPrice : product.Price,
            FullDescription = product.FullDescription,
            Gallery = gallery,
            Gtin = product.Gtin,
            ProductPrice = product.Price,
            ShortDescription = product.ShortDescription,
            ShowStockQuantity = product.DisplayStockQuantity,
            StockQuantity = product.DisplayStockQuantity ? product.StockQuantity : 0,
            Sku = product.Sku,
            Mpn = product.ManufacturerPartNumber,
            Slug = slug,
            //attributes = productAttributes,
            Reviews = reviews,
            Variants = variants,
            //unit =
        };
    }

    private async Task<string> GetBannerAsync(Product product, long epoch)
    {
        if (product.MarkAsNew &&
                (product.MarkAsNewEndDateTimeUtc == null ||
                (epoch - new DateTimeOffset(product.MarkAsNewEndDateTimeUtc.Value).ToUnixTimeSeconds() > 0)))
            return "new";

        var pd = await _productService.GetAllDiscountsAppliedToProductAsync(product.Id);

        return pd?.Any() == false ? "sale" : "none";
    }
    private async Task<object> GetProductVariantsAsync(Product product, int languageId)
    {
        var res = new List<object>();
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);

        foreach (var attribute in productAttributeMapping)
        {
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

            var variant = new
            {
                id = attribute.Id,
                //ProductAttributeId = attribute.ProductAttributeId,
                controlType = attribute.AttributeControlType.ToString(),
                defaultValue = await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue, languageId),
                description = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Description, languageId),
                hasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml),
                required = attribute.IsRequired,
                name = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Name, languageId),
                options = new List<object>(),
                textPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt, languageId),
            };

            if (attribute.ShouldHaveValues())
            {
                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var picture = await _pictureService.GetPictureByIdAsync(attributeValue.ImageSquaresPictureId);

                    //(var fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (var imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.ImageSquarePictureSize);

                    var item = await _mediaPreperar.ToMediaItemAsync(picture);
                    var name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name, languageId);

                    var vo = new
                    {
                        id = attributeValue.Id,
                        colorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                        customerEntersQty = attributeValue.CustomerEntersQty,
                        image = imageUrl,
                        quantity = attributeValue.Quantity,
                        name,
                        preSelected = attributeValue.IsPreSelected,
                    };
                    variant.options.Add(vo);
                }
            }

            res.Add(variant);
        }

        return res;
    }

    private async Task<IEnumerable<object>> GetProductReviewsAsync(Product product)
    {
        var reviews = await _productService.GetAllProductReviewsAsync(productId: product.Id, approved: true);
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

            var item = await _mediaPreperar.ToMediaItemAsync(picture, pp.DisplayOrder);

            cmis.Add(item);
        }

        var videos = await _videoService.GetVideosByProductIdAsync(product.Id);
        var productVideos = await _productService.GetProductVideosByProductIdAsync(product.Id);
        foreach (var v in productVideos)
        {
            var video = videos.FirstOrDefault(p => p.Id == v.VideoId);
            if (video == null)
                continue;

            cmis.Add(_mediaPreperar.ToMediaItem(video, v.DisplayOrder));
        }
        return cmis.ToList();
    }

    public async Task<ShoppingCartApiModel> ToShoppingCartApiModel(IEnumerable<ShoppingCartItem> cart)
    {
        var productIds = cart.Select(p => p.ProductId).ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);
        var productInfos = await ToProductApiModel(products);

        var items = new List<ShoppingCartItemApiModel>();
        var updatedDate = default(DateTime);

        foreach (var ci in cart)
        {
            var productAttribute = (await _productAttributeParser.ParseProductAttributeValuesAsync(ci.AttributesXml)).FirstOrDefault();
            var u = ci.CreatedOnUtc > ci.UpdatedOnUtc ? ci.CreatedOnUtc : ci.UpdatedOnUtc;

            if (updatedDate < u)
                updatedDate = u;

            items.Add(new()
            {
                Quantity = ci.Quantity,
                VariantId = productAttribute?.ProductAttributeMappingId ?? 0,
                ProductInfo = productInfos.FirstOrDefault(x => x.Id == ci.ProductId),
                ProductId = ci.ProductId,
                OptionId = productAttribute?.Id ?? 0,
            });
        }
        return new()
        {
            Items = items,
            UpdatedOnUtc = items.Count == 0? 0: new DateTimeOffset(updatedDate).ToUnixTimeSeconds(),
        };
    }
}
