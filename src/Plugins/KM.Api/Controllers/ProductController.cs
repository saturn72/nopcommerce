using Nop.Core.Domain.Catalog;
using KM.Api.Factories;

namespace KM.Api.Controllers;

[Route("api/product")]
public class ProductController : KmApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductApiFactory _productApiFactory;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;

    public ProductController(
        IProductService productService,
        IProductApiFactory productApiFactory,
        IStaticCacheManager cache,
        IStoreContext storeContext)
    {
        _productService = productService;
        _productApiFactory = productApiFactory;
        _staticCacheManager = cache;
        _storeContext = storeContext;
    }

    [HttpGet]
    public async Task<IActionResult> Query(
        [FromQuery(Name = "q")] string? keywords,
        [FromQuery] int offset = 0,
        [FromQuery] int pageSize = 50)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var prefix = $"{NopEntityCacheDefaults<Product>.Prefix}query.";
        var key = $"{prefix}storeid:{store.Id}-{nameof(offset)}:{offset}-{nameof(pageSize).ToLower()}:{pageSize}-{nameof(keywords)}:{keywords}";
        var ck = new CacheKey(key,
            prefix,
            //NopEntityCacheDefaults<Product>.Prefix,
            //NopEntityCacheDefaults<Product>.ByIdPrefix,
            NopEntityCacheDefaults<Product>.ByIdsPrefix,
            NopEntityCacheDefaults<Product>.AllPrefix);

        var data = await _staticCacheManager.GetAsync(ck, aquireProductQuery);
        return ToJsonResult(new { products = data });

        async Task<object> aquireProductQuery()
        {
            var products = await _productService.SearchProductsAsync(
                offset,
                pageSize,
                storeId: store.Id,
                keywords: keywords,
                searchDescriptions: true,
                searchManufacturerPartNumber: true,
                searchProductTags: true,
                searchSku: true,
                orderBy: ProductSortingEnum.Position
                );

            return await _productApiFactory.ToProductApiModel(products);
        }
    }
}
