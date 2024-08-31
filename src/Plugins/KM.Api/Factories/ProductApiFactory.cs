using KM.Api.Models.Catalog;
using KM.Api.Models.Media;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace KM.Api.Factories;

public class ProductApiFactory : IProductApiFactory
{
    private readonly IProductService _productService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly MediaConvertor _mediaConvertor;
    private readonly IProductAttributeFormatter _productAttributeFormatter;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;

    public ProductApiFactory(
        IProductService productService,
        IProductAttributeParser productAttributeParser,
        MediaConvertor mediaFactory,
        IProductAttributeFormatter productAttributeFormatter,
        IProductModelFactory productModelFactory,
        IPriceFormatter priceFormatter,
        IWorkContext workContext,
        IStoreContext storeContext)
    {
        _productService = productService;
        _productAttributeParser = productAttributeParser;
        _mediaConvertor = mediaFactory;
        _productAttributeFormatter = productAttributeFormatter;
        _productModelFactory = productModelFactory;
        _priceFormatter = priceFormatter;
        _workContext = workContext;
        _storeContext = storeContext;
    }

    public async Task<IEnumerable<ProductInfoApiModel>> ToProductInfoApiModel(IEnumerable<Product> products)
    {
        var m = new List<ProductInfoApiModel>();
        foreach (var p in products)
        {
            var model = await _productModelFactory.PrepareProductDetailsModelAsync(p, null, false);
            m.Add(await ToProductInfo(model, p));
        }
        return m;
    }
    private async Task<ProductInfoApiModel> ToProductInfo(ProductDetailsModel productDetails, Product product)
    {
        var banners = await GetBannerAsync(productDetails, product);
        var gallery = GetProductGalleryAsync(productDetails);
        var variants = await GetProductVariantsAsync(productDetails, product);
        return new()
        {
            Id = productDetails.Id,
            Banners = banners,
            DisplayIndex = product.DisplayOrder,
            FullDescription = productDetails.FullDescription,
            Mpn = productDetails.ManufacturerPartNumber,
            Name = productDetails.Name,
            Price = productDetails.ProductPrice.PriceValue,
            PriceText = productDetails.ProductPrice.Price,
            PriceOld = productDetails.ProductPrice.OldPriceValue,
            PriceOldText = productDetails.ProductPrice.OldPrice,
            PriceWithDiscount = productDetails.ProductPrice.PriceWithDiscountValue,
            PriceWithDiscountText = productDetails.ProductPrice.PriceWithDiscount,
            Gallery = gallery,
            Gtin = productDetails.Gtin,
            Reviews = productDetails.ProductReviews,
            ShortDescription = productDetails.ShortDescription,
            ShowOnHomePage = product.ShowOnHomepage,
            ShowStockQuantity = product.DisplayStockQuantity,
            StockAvailability = productDetails.StockAvailability,
            Sku = productDetails.Sku,
            Slug = productDetails.SeName,
            Variants = variants,
        };
    }

    private async Task<IEnumerable<ProductInfoApiModel.ProductBanner>> GetBannerAsync(ProductDetailsModel productDetails, Product product)
    {
        var i = 0;
        var res = new List<ProductInfoApiModel.ProductBanner>();
        //check old-price-value vs price-value
        //check price-with-discountsvalue vs price-value
        //check discounts
        //Add "special-offer" banner

        if (productDetails.ProductPrice.PriceValue < productDetails.ProductPrice.PriceWithDiscountValue)
            res.Add(new() { Priority = i++, Key = "sale" });

        if (product.MarkAsNew &&
            (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
            (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow))
            res.Add(new() { Priority = i++, Key = "new" });

        var pd = await _productService.GetAllDiscountsAppliedToProductAsync(product.Id);
        if (pd.Count > 0)
            res.Add(new() { Priority = i++, Key = "promotion" });

        return res;
    }
    private async Task<IEnumerable<ProductInfoApiModel.Variant>> GetProductVariantsAsync(ProductDetailsModel productDetails, Product product)
    {
        var errors = new List<string>();
        var res = new List<ProductInfoApiModel.Variant>();
        foreach (var attribute in productDetails.ProductAttributes)
        {
            var variant = new ProductInfoApiModel.Variant()
            {
                Id = attribute.Id,
                //ProductAttributeId = attribute.ProductAttributeId,
                ControlType = attribute.AttributeControlType.ToString(),
                DefaultValue = attribute.DefaultValue,
                Description = attribute.Description,
                HasCondition = attribute.HasCondition,
                Required = attribute.IsRequired,
                Name = attribute.Name,
                Options = new List<ProductInfoApiModel.Variant.Option>(),
                TextPrompt = attribute.TextPrompt,
            };

            if (attribute.Values.Count() > 0)
            {
                //values
                foreach (var attributeValue in attribute.Values)
                {
                    var form = new FormCollection(new() { { $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}", attributeValue.Id.ToString() }, });
                    var attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);
                    var displayText = await _productAttributeFormatter.FormatAttributesAsync(
                        product,
                        attributesXml,
                        await _workContext.GetCurrentCustomerAsync(),
                        await _storeContext.GetCurrentStoreAsync(),
                        renderPrices: false);

                    var p = await calculatePrice(
                        productDetails.ProductPrice.PriceValue,
                        attributeValue.PriceAdjustmentUsePercentage,
                        attributeValue.PriceAdjustmentValue);

                    var po = await calculatePrice(
                        productDetails.ProductPrice.OldPriceValue,
                        attributeValue.PriceAdjustmentUsePercentage,
                        attributeValue.PriceAdjustmentValue);

                    var pwd = await calculatePrice(
                        productDetails.ProductPrice.PriceWithDiscountValue,
                        attributeValue.PriceAdjustmentUsePercentage,
                        attributeValue.PriceAdjustmentValue);

                    var vo = new ProductInfoApiModel.Variant.Option
                    {
                        Id = attributeValue.Id,
                        ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                        CustomerEntersQty = attributeValue.CustomerEntersQty,
                        DisplayText = displayText,
                        Image = _mediaConvertor.ToGalleryItemModel(attributeValue.ImageSquaresPictureModel, 0),
                        Name = attributeValue.Name,
                        Price = p.value,
                        PriceText = p.text,
                        PriceOld = po.value,
                        PriceOldText = po.text,
                        PriceWithDiscount = pwd.value,
                        PriceWithDiscountText = pwd.text,
                        PreSelected = attributeValue.IsPreSelected,
                        Quantity = attributeValue.Quantity,
                    };

                    variant.Options.Add(vo);
                }
            }
            res.Add(variant);
        }
        return res;

        async Task<(decimal? value, string text)> calculatePrice(
            decimal? basePrice,
            bool usePercentage,
            decimal priceAdjustment)
        {
            if (basePrice == null)
                return (null, null);

            var value = usePercentage ?
                basePrice * (1 + priceAdjustment) :
                basePrice + priceAdjustment;

            var text = await _priceFormatter.FormatPriceAsync(value.Value);

            return (value, text);
        }
    }

    private IEnumerable<GalleryItemModel> GetProductGalleryAsync(ProductDetailsModel productDetails)
    {
        var images = new List<PictureModel>(new[] { productDetails.DefaultPictureModel });
        images.AddRange(productDetails.PictureModels);
        var res = new List<GalleryItemModel>();

        for (var i = 0; i < images.Count(); i++)
        {
            var p = images.ElementAt(i);
            res.Add(_mediaConvertor.ToGalleryItemModel(p, i));
        }


        for (var i = 0; i < productDetails.VideoModels.Count(); i++)
        {
            var v = productDetails.VideoModels.ElementAt(i);
            res.Add(_mediaConvertor.ToGalleryItemModel(v, i));
        }

        return res;
    }

    public async Task<ShoppingCartApiModel> ToShoppingCartApiModel(IEnumerable<ShoppingCartItem> cart)
    {
        var productIds = cart.Select(p => p.ProductId).ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);
        var productInfos = await ToProductInfoApiModel(products);

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
            UpdatedOnUtc = items.Count == 0 ? 0 : new DateTimeOffset(updatedDate).ToUnixTimeSeconds(),
        };
    }
}
