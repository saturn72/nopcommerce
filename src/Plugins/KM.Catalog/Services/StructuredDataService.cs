using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Services.Customers;
using Nop.Services.Directory;

namespace Km.Catalog.Services;
public class StructuredDataService : IStructuredDataService
{
    private readonly IProductService _productService;
    private readonly IProductTagService _productTagService;
    private readonly ICategoryService _categoryService;
    private readonly ICustomerService _customerService;
    private readonly ICurrencyService _currencyService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IManufacturerService _manufacturerService;
    private readonly IVendorService _vendorService;

    public StructuredDataService(
        IProductService productService,
        IProductTagService productTagService,
        ICategoryService categoryService,
        ICustomerService customerService,
        ICurrencyService currencyService,
        IStaticCacheManager staticCacheManager,
        IProductAttributeService productAttributeService,
        IManufacturerService manufacturerService,
        IVendorService vendorService)
    {
        _productService = productService;
        _productTagService = productTagService;
        _categoryService = categoryService;
        _customerService = customerService;
        _currencyService = currencyService;
        _productAttributeService = productAttributeService;
        _staticCacheManager = staticCacheManager;
        _manufacturerService = manufacturerService;
        _vendorService = vendorService;
    }
    public async Task<object> GenerateProductStructuredDataAsync(Product product, Currency currency)
    {
        var review = await GenerateProductReviewsStructuredDataAsync(product, currency);

        var productTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);
        var tags = string.Join(',', productTags.Select(x => x.Name).ToArray());
        var manufacturerOrVendor = await GetManufacturerAsync(product);

        return new
        {
            @context = "https://schema.org/",
            @type = "Product",
            additionalProperty = await GetAllProductAttributesWithValuesAsync(product),
            aggregateRating = product.ApprovedRatingSum,
            award = await GetProductAttributeValueAsync("award", product),
            brand = await GetProductAttributeValueAsync("brand", product),
            category = await GetProductCategoriesAsync(product.Id),
            color = await GetProductAttributeValueAsync("color", product),
            depth = GetMeasurementUnit(product.Length),
            description = product.ShortDescription ?? product.ShortDescription,
            gtin = product.Gtin,
            //hasMerchantReturnPolicy = return policy
            height = GetMeasurementUnit(product.Height),
            itemCondition = await getItemCondition(),
            keywords = tags,
            manufacturer = manufacturerOrVendor,
            material = await GetProductAttributeValueAsync("material", product),
            mpn = product.ManufacturerPartNumber,
            name = product.Name,
            offers = await GenerateProductOfferStructuredDataAsync(product, currency),
            productId = product.Id,
            review = review,
            sku = product.Sku,
            weight = GetMeasurementUnit(product.Weight),
            width = GetMeasurementUnit(product.Width),
        };

        async Task<string?> getItemCondition()
        {
            var (_, condition) = await GetProductAttributeValueAsync("condition", product);

            switch (condition)
            {
                case "New":
                    return "NewCondition";
                case "Used":
                    return "UsedCondition";
                default:
                    return default;
            }
            //            "DamagedCondition"
            //"RefurbishedCondition"
        }
    }

    private object? GetMeasurementUnit(decimal value)
    {
        if (value == default)
            return null;

        return new
        {
            @type = "QuantitativeValue",
            value = value,
            unitCode = "CMT",
        };
    }

    private async Task<string?> GetManufacturerAsync(Product product)
    {
        var productManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
        if (productManufacturers.NotNullAndNotEmpty())
        {
            var manufacturerIds = productManufacturers.Select(m => m.ManufacturerId).ToArray();
            var manufacturers = await _manufacturerService.GetManufacturersByIdsAsync(manufacturerIds);
            return string.Join(',', manufacturers.ToList());
        }
        return (await _vendorService.GetVendorByIdAsync(product.VendorId))?.Name;
    }

    private async Task<IEnumerable<object>> GenerateProductReviewsStructuredDataAsync(Product product, Currency currency)
    {
        var reviews = await _productService.GetAllProductReviewsAsync(
            productId: product.Id,
            approved: true,
            pageSize: int.MaxValue);


        return reviews.Select(async r =>
        {
            var customer = await _customerService.GetCustomerByIdAsync(r.CustomerId);
            return new
            {
                @context = "https://schema.org",
                @type = "Review",
                author = new
                {
                    @type = "Person",
                    additionalName = customer.Username,
                    email = customer.Email,
                    familyName = customer.LastName,
                    givenName = customer.FirstName,
                    name = customer.SystemName
                },
                offers = await GenerateProductOfferStructuredDataAsync(product, currency),
                reviewBody = r.ReviewText,
                reviewRating = new
                {
                    @type = "Rating",
                    bestRating = 5,
                    ratingValue = r.Rating,
                    worstRating = 1,
                }
            };
        });
    }

    private async Task<object> GenerateProductOfferStructuredDataAsync(Product product, Currency currency)
    {
        return new
        {
            @type = new[] { "Offer" },
            acceptedPaymentMethod = new[]
            {
                "http://purl.org/goodrelations/v1#ByBankTransferInAdvance",
                "http://purl.org/goodrelations/v1#Cash",
                "http://purl.org/goodrelations/v1#COD",
                "http://purl.org/goodrelations/v1#GoogleCheckout",
                "http://purl.org/goodrelations/v1#PayPal",
                "bit",
                "paybox"
            },
            aggregateRating = product.ApprovedRatingSum,
            availability = getAvailability(),
            availabilityEnds = product.AvailableEndDateTimeUtc.ToIso8601(),
            availabilityStarts = product.AvailableStartDateTimeUtc.ToIso8601(),
            businessFunction = getBusinessFunction(),
            category = await GetProductCategoriesAsync(product.Id),
            deliveryLeadTime = new
            {
                minValue = 0,
                maxValue = 2,
            },
            gtin = product.Gtin,
            mpn = product.ManufacturerPartNumber,
            price = product.Price,
            priceCurrenty = currency.CurrencyCode,
            sku = await GetProductAttributeValueAsync("sku", product),
            validFrom = product.AvailableStartDateTimeUtc.ToIso8601(),
            validThrough = product.AvailableStartDateTimeUtc.ToIso8601(),

        };

        IEnumerable<string> getBusinessFunction()
        {
            return product.IsRental ?
                new[] { "http://purl.org/goodrelations/v1#Sell", "http://purl.org/goodrelations/v1#LeaseOut" } :
                new[] { "http://purl.org/goodrelations/v1#Sell" };
        }

        string getAvailability()
        {
            if (product.AvailableForPreOrder == true)
                return "PreOrder";

            if (!product.Published)
                return "Discontinued";

            if (product.StockQuantity > product.MinStockQuantity)
                return "InStock";

            if (product.BackorderMode != BackorderMode.NoBackorders)
                return "BackOrder";

            return "OutOfStock";
            //"InStoreOnly"}
            //"LimitedAvailability"}
            //"OnlineOnly"}
            //"PreSale"}
            //"SoldOut"}
        }
    }


    private async Task<IEnumerable<string>> GetProductCategoriesAsync(int productId)
    {
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);
        var categoryIds = productCategories.Select(pc => pc.CategoryId).ToArray();
        var categories = await _categoryService.GetCategoriesByIdsAsync(categoryIds);
        var result = new List<string>();

        foreach (var item in categories)
        {
            var text = await _categoryService.GetFormattedBreadCrumbAsync(item, separator: ">");
            result.Add(text);
        }
        return result;
    }

    private async Task<IEnumerable<(string, string?)>> GetAllProductAttributesWithValuesAsync(Product product)
    {
        var allProductAttributes = await _productAttributeService.GetAllProductAttributesAsync();
        var res = new List<(string, string?)>();

        foreach (var pa in allProductAttributes)
        {
            var value = await GetProductAttributeValueAsync(product.Id, pa);

            res.Add((pa.Name, value));
        }

        return res;
    }

    private async Task<(string, string?)> GetProductAttributeValueAsync(string attributeName, Product product)
    {
        var allProductAttributes = await _productAttributeService.GetAllProductAttributesAsync();
        var productAttribute = allProductAttributes.FirstOrDefault(a => a.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));

        if (productAttribute == default)
            return (attributeName, null);

        var value = await GetProductAttributeValueAsync(product.Id, productAttribute);
        return (attributeName, value);
    }
    private async Task<string?> GetProductAttributeValueAsync(
        int productId,
        ProductAttribute productAttribute)
    {
        var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
        var curMap = pams.FirstOrDefault(x => x.ProductAttributeId == productAttribute.Id);

        if (curMap == null)
            return default;

        var pav = await _productAttributeService.GetProductAttributeValuesAsync(curMap.Id);
        return string.Join(',', pav.Select(s => s.Name).ToArray());
    }

    public Task<object> GenerateStoreStructuredDataAsync(Store store)
    {
        var sds = new
        {
            @context = "https://schema.org",
            @type = new[] { "FoodEstablishment", "Store" },
        };

        return Task.FromResult(sds);

        //        o["address"] = new Dictionary<string, string> {
        //            { "@type", "Text" },
        //        store.CompanyAddress
        //            {"streetAddress", "148 W 51st St Suit 42 Unit 7" },
        //  {"addressLocality", "New York"},
        // { "addressRegion", "NY"},
        //{  "postalCode", "10019"},
        //  {"addressCountry", "US"}
        //};

        //local business
        //image
        //video??
        //Profile page
        //FAQ
        //Review
        //Sitelink search box
        //Video
        //specify products: https://developers.google.com/search/docs/appearance/structured-data/carousel
        throw new NotImplementedException();
    }
}
