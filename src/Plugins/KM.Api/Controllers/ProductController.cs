using KM.Api.Factories;

namespace KM.Api.Controllers;

[Route("api/product")]
public class ProductController : KmApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductApiFactory _productApiFactory;
    private readonly IStoreContext _storeContext;

    public ProductController(
        IProductService productService,
        IProductApiFactory productApiFactory,
        IStoreContext storeContext)
    {
        _productService = productService;
        _productApiFactory = productApiFactory;
        _storeContext = storeContext;
    }

    [HttpGet]
    public async Task<IActionResult> Query(
        [FromQuery(Name = "q")] string? keywords,
        [FromQuery] int offset = 0,
        [FromQuery] int pageSize = 50)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
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

        var data = await _productApiFactory.ToProductInfoApiModel(products);
        return ToJsonResult(new { products = data });
    }
}
