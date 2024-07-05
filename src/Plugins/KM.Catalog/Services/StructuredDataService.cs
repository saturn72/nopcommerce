using Nop.Core.Domain.Directory;
using Nop.Services.Customers;

namespace KM.Catalog.Services;
public class StructuredDataService : IStructuredDataService
{
    private readonly IProductService _productService;
    private readonly IProductTagService _productTagService;
    private readonly ICategoryService _categoryService;
    private readonly ICustomerService _customerService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IVendorService _vendorService;

    private static readonly string[] ProductAdultConsidirationKeys = new[]{
                "AlcoholConsideration",
                "DangerousGoodConsideration",
                "HealthcareConsideration",
                "NarcoticConsideration",
                "ReducedRelevanceForChildrenConsideration",
                "SexualContentConsideration",
                "TobaccoNicotineConsideration",
                "UnclassifiedAdultConsideration",
                "ViolenceConsideration",
                "WeaponConsideration"
            };
    public StructuredDataService(
        IProductService productService,
        IProductTagService productTagService,
        ICategoryService categoryService,
        ICustomerService customerService,
        IProductAttributeService productAttributeService,
        IManufacturerService manufacturerService,
        IVendorService vendorService)
    {
        _productService = productService;
        _productTagService = productTagService;
        _categoryService = categoryService;
        _customerService = customerService;
        _productAttributeService = productAttributeService;
        _manufacturerService = manufacturerService;
        _vendorService = vendorService;
    }
    public async Task<object> GenerateProductStructuredDataAsync(Product product, Currency currency)
    {
        var (reviews, positiveNotes, negativeNotes) = await GenerateProductReviewsStructuredDataAsync(product, currency);

        var productTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);
        var tags = string.Join(',', productTags.Select(x => x.Name).ToArray());
        var manufacturerOrVendor = await GetManufacturerAsync(product);

        var allProductAttributes = await GetAllProductAttributesWithValuesAsync(product);
        return new
        {
            @context = "https://schema.org/",
            @type = "Product",
            additionalProperty = allProductAttributes,
            aggregateRating = product.ApprovedRatingSum,
            alternateName = getAttributeValue("alias"),
            award = getAttributeValue("award"),
            brand = getAttributeValue("brand"),
            category = await GetProductCategoriesAsync(product.Id),
            color = getAttributeValue("color"),
            depth = GetMeasurementUnit(product.Length),
            description = product.ShortDescription ?? product.FullDescription,
            gtin = product.Gtin,
            hasAdultConsideration = getAdultConsidiration(),
            height = GetMeasurementUnit(product.Height),
            //hasCeritication = TBD
            //hasMerchantReturnPolicy = return policy
            identifier = product.Id,
            isFamilyFriendly = getAttributeValue("family friendly"),
            itemCondition = getItemCondition(),
            keywords = tags,
            manufacturer = manufacturerOrVendor,
            material = getAttributeValue("material"),
            model = getAttributeValue("model"),
            mpn = product.ManufacturerPartNumber,
            name = product.Name,
            negativeNotes = negativeNotes,
            offers = await GenerateProductOfferStructuredDataAsync(product, currency),
            pattern = getAttributeValue("pattern"),
            positiveNotes = positiveNotes,
            productID = product.Id,
            review = reviews,
            size = getAttributeValue("size"),
            sku = product.Sku,
            weight = GetMeasurementUnit(product.Weight),
            width = GetMeasurementUnit(product.Width),
        };

        string? getAttributeValue(string attributeName)
        {
            var (_, value) = allProductAttributes
                .FirstOrDefault(x => x.name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase));

            return value;
        }

        IEnumerable<string>? getAdultConsidiration()
        {
            var res = new List<string>();
            foreach (var c in ProductAdultConsidirationKeys)
            {
                if (getAttributeValue(c) != default)
                    res.Add(c);
            }

            return res.NotNullAndNotEmpty() ? res : default;
        }

        string? getItemCondition()
        {
            var condition = getAttributeValue("condition");

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

    private async Task<(IEnumerable<object> reviews, object positiveNotes, object negativeNotes)> GenerateProductReviewsStructuredDataAsync(Product product, Currency currency)
    {
        var allReviews = await _productService.GetAllProductReviewsAsync(
            productId: product.Id,
            approved: true,
            pageSize: int.MaxValue);

        var reviews = new List<object>();
        var negativeNotesItems = new List<object>();
        var positiveNotesItems = new List<object>();

        for (var i = 0; i < allReviews.Count; i++)
        {
            var r = allReviews.ElementAt(i);
            if (r.Rating == 0)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(r.CustomerId);
            var reviewSD = new
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

            var col = r.Rating >= 3 ? positiveNotesItems : negativeNotesItems;
            var item = new
            {
                @type = "ListItem",
                position = col.Count + 1,
                name = r.ReviewText,
            };
            col.Add(item);
            reviews.Add(r);
        }

        var negativeNotes = new
        {
            @type = "ItemList",
            itemListElement = negativeNotesItems.ToArray(),
        };

        var positiveNotes = new
        {
            @type = "ItemList",
            itemListElement = positiveNotesItems.ToArray(),
        };

        return (reviews, positiveNotes, negativeNotes);
    }

    private async Task<object> GenerateProductOfferStructuredDataAsync(Product product, Currency currency)
    {
        return new
        {
            @type = "Offer",
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
            priceCurrenty = currency?.CurrencyCode,
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

    private async Task<IEnumerable<(string name, string? value)>> GetAllProductAttributesWithValuesAsync(Product product)
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
            //this field should be added to store/vendor configuration
            //list is located here: https://schema.org/LocalBusiness#subtypes
            @type = new[] { "FoodEstablishment", "Store" },
            //currenciesAccepted = use _storeMappingService to extract supported currencies,
            address = store.CompanyAddress,
            openingHoursSpecification = getOpeningHoursSpecification(),
        };

        return Task.FromResult(sds as object);

        static object getOpeningHoursSpecification()
        {
            return new[]
            {
                new
                {
                    @type = "OpeningHoursSpecification",
                    dayOfWeek = new[]
                    {
                        "Sunday",
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                    },
                    opens = "09:00",
                    closes = "23:00"
                },
                new
                {
                    @type = "OpeningHoursSpecification",
                    dayOfWeek = new[]
                    {
                        "Friday",
                    },
                    opens = "09:00",
                    closes = "13:30"
                },
            };
        }
    }
}
