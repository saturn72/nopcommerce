namespace KM.Api.Controllers;


[Route("api/shopping-cart")]
public class ShoppingCartController : KmApiControllerBase
{
    private readonly IExternalUsersService _userService;
    private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreService _storeService;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;

    public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IExternalUsersService userService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreService storeService,
            IProductService productService,
            IStoreMappingService storeMappingService)
    {
        _shoppingCartService = shoppingCartService;
        _userService = userService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _storeService = storeService;
        _productService = productService;
        _storeMappingService = storeMappingService;
    }
    private async Task UpdateShoppingCartAsync(int storeId, Customer customer, IEnumerable<ShoppingCartItem> items)
    {
        //var customer = await _userService.GetCustomerByExternalUserIdAsync(userId);
        //var customer = await _customerService .GetCustomerByExternalUserIdAsync(userId);
        if (customer == null)
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, storeId: storeId);
        var tasks = new List<Task>();

        var productIds = items.Select(x => x.ProductId).ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);

        var toDelete = cart.Where(c => !productIds.Contains(c.ProductId)).ToList();
        foreach (var td in toDelete)
        {
            tasks.Add(_shoppingCartService.DeleteShoppingCartItemAsync(td));
            cart.Remove(td);
        }

        foreach (var ci in items)
        {
            var a = cart.FirstOrDefault(x => x.ProductId == ci.ProductId);
            //added
            if (a == null)
            {
                if (ci.Quantity > 0)
                {
                    var product = products.First(p => p.Id == ci.ProductId);
                    var at = _shoppingCartService.AddToCartAsync(
                        customer,
                        product,
                        ShoppingCartType.ShoppingCart,
                        storeId,
                        quantity: ci.Quantity);
                    tasks.Add(at);
                }
                continue;
            }

            //no change
            if (a.Quantity == ci.Quantity)
                continue;
            //removed
            if (ci.Quantity <= 0)
            {
                tasks.Add(_shoppingCartService.DeleteShoppingCartItemAsync(a));
                continue;
            }

            //updated
            var ut = _shoppingCartService.UpdateShoppingCartItemAsync(
                customer,
                a.Id,
                a.AttributesXml,
                a.CustomerEnteredPrice,
                a.RentalStartDateUtc,
                a.RentalEndDateUtc,
                ci.Quantity);
            tasks.Add(ut);
        }
        await Task.WhenAll(tasks);

    }
    [HttpPut]
    public async Task<IActionResult> AddOrCreateShoppingCart([FromBody] ShoppingCartApiModel model)
    {
        if (!ModelState.IsValid || !await ValidateShoppingCartModelAsync(model))
            return BadRequest();

        var customer = await _userService.GetCustomerByExternalUserIdAsync(model.UserId);
        if (customer == default)
            return BadRequest();

        var items = model.Items.Select(i => new ShoppingCartItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
        });
        await UpdateShoppingCartAsync(model.StoreId, customer, items);

        return Ok();
    }


    [HttpGet("{storeId}/{userId}")]
    public async Task<IActionResult> GetCartByStoreIdAndUserIdAsync(int storeId, string userId)
    {
        var customer = await _userService.GetCustomerByExternalUserIdAsync(userId);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId: storeId);

        var items = cart.Select(ci => new
        {
            productId = ci.ProductId,
            quantity = ci.Quantity,
            updatedOnUtc = new DateTimeOffset(ci.UpdatedOnUtc).ToUnixTimeSeconds(),
            createdOnUtc = new DateTimeOffset(ci.CreatedOnUtc).ToUnixTimeSeconds()
        }).ToArray();

        return ToJsonResult(new { items });
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateShoppingCartAsync([FromBody] ShoppingCartApiModel model)
    {
        if (!ModelState.IsValid || !await ValidateShoppingCartModelAsync(model))
            return BadRequest();

        var customer = await _userService.GetCustomerByExternalUserIdAsync(model.UserId);
        if (customer == default)
            return BadRequest();

        //prevent update in background
        //_cache.Remove(GetUserCartCacheKey(model.StoreId, model.UserId));
        var items = model.Items.Select(i => new ShoppingCartItem { ProductId = i.ProductId, Quantity = i.Quantity });
        await UpdateShoppingCartAsync(model.StoreId, customer, items);

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId: model.StoreId);
        var scm = new ShoppingCartModel();
        scm = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(scm, cart);

        var body = new
        {
            items = scm.Items
        };
        return ToJsonResult(body);
    }
    private async Task<bool> ValidateShoppingCartModelAsync(ShoppingCartApiModel model)
    {
        var store = await _storeService.GetStoreByIdAsync(model.StoreId);
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
                if (storeMap.All(sm => sm.StoreId != model.StoreId))
                    return false;
            }
        }

        var customer = await _userService.GetCustomerByExternalUserIdAsync(model.UserId);
        return true;
    }
}
