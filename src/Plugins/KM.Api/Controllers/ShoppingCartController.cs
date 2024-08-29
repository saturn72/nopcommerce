using KM.Api.Factories;

namespace KM.Api.Controllers;


[Route("api/shopping-cart")]
public class ShoppingCartController : KmApiControllerBase
{
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IProductApiFactory _productApiFactory;
    private readonly IShoppingCartFactory _shoppingCartFactory;

    public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductAttributeParser productAttributeParser,
            IProductApiFactory productApiFactory,
            IShoppingCartFactory shoppingCartFactory)
    {
        _shoppingCartService = shoppingCartService;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
        _storeContext = storeContext;
        _productAttributeParser = productAttributeParser;
        _productApiFactory = productApiFactory;
        _shoppingCartFactory = shoppingCartFactory;
    }
    private async Task SaveShoppingCartAsync(
        Customer customer,
        Store store,
        IList<ShoppingCartItem> items,
        List<string> errors)
    {
        if (items.Count == 0)
        {
            await _shoppingCartService.ClearShoppingCartAsync(customer, store.Id);
            return;
        }

        var tasks1 = new List<Task<IList<string>>>();
        var tasks2 = new List<Task>();
        var userCart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
        var productIds = items.Select(x => x.ProductId).ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);

        foreach (var item in items)
        {
            var product = products.FirstOrDefault(x => x.Id == item.ProductId);
            if (product == default)
                continue;

            var usci = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(
                userCart,
                ShoppingCartType.ShoppingCart,
                product,
                item.AttributesXml);

            //item not exist in cart - update
            if (usci == default)
            {
                var addTask = _shoppingCartService.AddToCartAsync(
                    customer,
                    product,
                    ShoppingCartType.ShoppingCart,
                    store.Id,
                    item.AttributesXml,
                    item.CustomerEnteredPrice,
                    item.RentalStartDateUtc,
                    item.RentalEndDateUtc,
                    item.Quantity,
                    true);
                tasks1.Add(addTask);
                continue;
            }
            //exist in cart - delete or update

            if (item.Quantity != 0)
            {
                var updateTask = _shoppingCartService.UpdateShoppingCartItemAsync(
                    customer, usci.Id,
                    item.AttributesXml,
                    item.CustomerEnteredPrice,
                    item.RentalStartDateUtc,
                    item.RentalEndDateUtc,
                    item.Quantity,
                    true);

                tasks1.Add(updateTask);
            }
        }

        //delete all items that were removed from incoming cart
        foreach (var uc in userCart)
        {
            var product = products.FirstOrDefault(x => x.Id == uc.ProductId);
            if (product == default)
            {
                tasks2.Add(_shoppingCartService.DeleteShoppingCartItemAsync(uc));
                continue;
            }

            var exist = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(
                items,
                ShoppingCartType.ShoppingCart,
                product,
                uc.AttributesXml);

            if (exist == default || exist.Quantity == 0)
                tasks2.Add(_shoppingCartService.DeleteShoppingCartItemAsync(uc));
        }

        await Task.WhenAll(tasks1);

        foreach (var task in tasks1)
            errors.AddRange(await task);

        await Task.WhenAll(tasks2);
    }

    [HttpPut]
    public async Task<IActionResult> AddOrCreateShoppingCart([FromBody] ShoppingCartApiModel model)
    {
        if (!ModelState.IsValid || !await ValidateShoppingCartModelAsync(model))
            return BadRequest();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == default)
            return BadRequest();

        var store = await _storeContext.GetCurrentStoreAsync();
        var errors = new List<string>();
        var items = await _shoppingCartFactory.ToShoppingCartItems(model.Items, errors);
        await SaveShoppingCartAsync(customer, store, items, errors);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetCartByStoreIdAndUserIdAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var cart = await _shoppingCartService.GetShoppingCartAsync(
            customer,
            ShoppingCartType.ShoppingCart,
            storeId: store.Id);

        var cartModel = await _productApiFactory.ToShoppingCartApiModel(cart);
        return ToJsonResult(cartModel);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateShoppingCartAsync([FromBody] ShoppingCartApiModel model)
    {
        if (!ModelState.IsValid || !await ValidateShoppingCartModelAsync(model))
            return BadRequest();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == default)
            return BadRequest();

        var store = await _storeContext.GetCurrentStoreAsync();
        var errors = new List<string>();
        var items = await _shoppingCartFactory.ToShoppingCartItems(model.Items, errors);
        await SaveShoppingCartAsync(customer, store, items, errors);
        
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId: store.Id);
        var ccm = await _shoppingCartFactory.PrepareCheckoutCartApiModelAsync(cart);
        return ToJsonResult(ccm);
    }
    private async Task<bool> ValidateShoppingCartModelAsync(ShoppingCartApiModel model)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        if (store == default)
            return false;

        var productIds = model.Items.Select(p => p.ProductId).ToArray();
        if (productIds.Any(pId => pId <= 0))
            return false;

        var products = (await _productService.GetProductsByIdsAsync(productIds)).Where(p => !p.Deleted);

        foreach (var product in products)
        {
            if (product.LimitedToStores)
            {
                var storeMap = await _storeMappingService.GetStoreMappingsAsync(product);
                if (storeMap.All(sm => sm.StoreId != store.Id))
                    return false;
            }
        }

        return true;
    }
}
