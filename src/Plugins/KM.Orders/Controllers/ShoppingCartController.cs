
namespace Km.Orders.Controllers
{
    [ApiController]
    [Area("api")]
    [Route("api/shopping-cart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IExternalUsersService _userService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;

        public ShoppingCartController(
            IExternalUsersService userService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreService storeService,
            IProductService productService,
            IStoreMappingService storeMappingService)
        {
            _userService = userService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _storeService = storeService;
            _productService = productService;
            _storeMappingService = storeMappingService;
        }

        [HttpPost]
        public async Task<IActionResult> CalculateShoppingCartAsync([FromBody] ShoppingCartApiModel incomingCart)
        {
            List<CartTransactionItemApiModel> exist = new();
            if (!ModelState.IsValid || !await validateModel())
                return BadRequest();

            var maps = await _userService.ProvisionUsersAsync(new[] { incomingCart.UserId });
            if (maps.IsNullOrEmpty())
                return BadRequest();
            var map = maps.First();

            var cart = exist.Select(i =>
                            new ShoppingCartItem
                            {
                                ProductId = i.ProductId,
                                Quantity = i.OrderedQuantity,
                                CustomerId = map.CustomerId,
                                StoreId = incomingCart.StoreId,
                                ShoppingCartType = ShoppingCartType.ShoppingCart,
                            }).ToList();

            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            return Ok(model);

            async Task<bool> validateModel()
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

                return true ;
            }
        }
    }
}
