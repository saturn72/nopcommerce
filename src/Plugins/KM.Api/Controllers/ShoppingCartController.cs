using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

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
    private readonly IMemoryCache _cache;

    private readonly MemoryCacheEntryOptions _mceo = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(3),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10),
    };
    private readonly PostEvictionCallbackRegistration _onCacheEntryEvicted;

    public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IExternalUsersService userService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreService storeService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IMemoryCache cache)
    {
        _shoppingCartService = shoppingCartService;
        _userService = userService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _storeService = storeService;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _cache = cache;

        _onCacheEntryEvicted = new()
        {
            EvictionCallback = (object key, object value, EvictionReason reason, object state) =>
        {
            if (value is not ShoppingCartApiModel t)
                return;

            switch (reason)
            {
                case EvictionReason.None:
                case EvictionReason.Replaced:
                    break;
                case EvictionReason.Removed:
                case EvictionReason.Expired:
                case EvictionReason.TokenExpired:
                case EvictionReason.Capacity:
                    _ = UpdateShoppingCartAsync(t);
                    break;
                default:
                    break;
            }
        },
            State = null
        };

        _mceo.PostEvictionCallbacks.Add(_onCacheEntryEvicted);
    }
    private async Task UpdateShoppingCartAsync(ShoppingCartApiModel data)
    {
        var customer = await _userService.GetCustomerByExternalUserIdAsync(data.UserId);
        if (customer == null)
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, storeId: data.StoreId);
        var tasks = new List<Task>();

        var newProducts = new List<ShoppingCartItemApiModel>();
        foreach (var ci in data.Items)
        {
            var a = cart.FirstOrDefault(x => x.ProductId == ci.ProductId);

            //product removed 
            if (a != null && ci.Quantity == 0)
            {
                tasks.Add(_shoppingCartService.DeleteShoppingCartItemAsync(a));
                continue;
            }

            //product added
            if (a == null)
            {
                newProducts.Add(ci);
                continue;
            }

            a.Quantity = ci.Quantity;
            var t = _shoppingCartService.UpdateShoppingCartItemAsync(
                customer,
                a.Id,
                a.AttributesXml,
                a.CustomerEnteredPrice,
                a.RentalStartDateUtc,
                a.RentalEndDateUtc,
                a.Quantity);
            tasks.Add(t);
        }

        if (newProducts.Any())
        {
            var ids = newProducts.Select(c => c.ProductId).ToArray();
            var products = await _productService.GetProductsByIdsAsync(ids);

            foreach (var np in newProducts)
            {
                var p = products.First(c => c.Id == np.ProductId);
                if (p.LimitedToStores)
                {
                    var storeMap = await _storeMappingService.GetStoreMappingsAsync(p);
                    if (storeMap.All(sm => sm.StoreId != data.StoreId))
                        continue;
                }

                tasks.Add(_shoppingCartService.AddToCartAsync(
                    customer,
                    p,
                    ShoppingCartType.ShoppingCart,
                    data.StoreId,
                    quantity: np.Quantity));
            }
        }
        await Task.WhenAll(tasks);
    }

    [HttpPut]
    public IActionResult AddOrCreateShoppingCart([FromBody] ShoppingCartApiModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var key = $"cart-{model.StoreId}:{model.UserId}";
        _cache.Set(key, model, _mceo);

        _ = _userService.GetCustomerByExternalUserIdAsync(model.UserId);
        return Accepted();
    }


    [HttpGet("{storeId}/{userId}")]
    public async Task<IActionResult> GetCartByStoreIdAndUserIdAsync(int storeId, string userId)
    {
        var customer = await _userService.GetCustomerByExternalUserIdAsync(userId);
        var cartItems = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId: storeId);

        var items = cartItems.Select(i => new
        {
            productId = i.ProductId,
            quantity = i.Quantity,
            updatedOnUtc = new DateTimeOffset(i.UpdatedOnUtc).ToUnixTimeSeconds(),
            createdOnUtc = new DateTimeOffset(i.CreatedOnUtc).ToUnixTimeSeconds()
        });

        var body = new
        {
            items
        };

        return ToJsonResult(body);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateShoppingCartAsync([FromBody] ShoppingCartApiModel incomingCart)
    {
        List<ShoppingCartItemApiModel> exist = new();
        if (!ModelState.IsValid || !await validateModelAsync())
            return BadRequest();

        var maps = await _userService.ProvisionUsersAsync(new[] { incomingCart.UserId });
        if (maps.IsNullOrEmpty())
            return BadRequest();
        var map = maps.First();

        var cart = exist.Select(i =>
                        new ShoppingCartItem
                        {
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                            CustomerId = map.CustomerId,
                            StoreId = incomingCart.StoreId,
                            ShoppingCartType = ShoppingCartType.ShoppingCart,
                        }).ToList();

        var model = new ShoppingCartModel();
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

        var body = new
        {
            items = model.Items
        };
        return ToJsonResult(body);

        async Task<bool> validateModelAsync()
        {
            var store = await _storeService.GetStoreByIdAsync(incomingCart.StoreId);
            if (store == default)
                return false;

            var productIds = incomingCart.Items.Select(p => p.ProductId).ToArray();
            var products = (await _productService.GetProductsByIdsAsync(productIds)).Where(p => !p.Deleted);

            foreach (var product in products)
            {
                if (product.LimitedToStores)
                {
                    var storeMap = await _storeMappingService.GetStoreMappingsAsync(product);
                    if (storeMap.All(sm => sm.StoreId != incomingCart.StoreId))
                        return false;
                }
                var cur = incomingCart.Items.FirstOrDefault(s => s.ProductId == product.Id);
                if (cur == default)
                    continue;
                exist.Add(cur);
            }

            return true;
        }
    }
}
