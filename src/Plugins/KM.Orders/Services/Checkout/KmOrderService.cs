
namespace KM.Orders.Services.Checkout;
public class KmOrderService : IKmOrderService
{
    private readonly IRepository<KmOrder> _kmOrderRepository;
    private readonly IExternalUsersService _externalUserService;
    private readonly IWorkContext _workContext;
    private readonly IStoreService _storeService;
    private readonly KmStoreContext _storeContext;
    private readonly ICustomerService _customerService;
    private readonly IAddressService _addressService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ISystemClock _systemClock;
    private readonly ILogger _logger;

    public KmOrderService(
        IRepository<KmOrder> kmOrderRepository,
        IExternalUsersService externalUserService,
        IWorkContext workContext,
        IStoreService storeService,
        IStoreContext storeContext,
        ICustomerService customerService,
        IAddressService addressService,
        IPaymentService paymentService,
        IOrderProcessingService orderProcessingService,
        IProductService productService,
        IStoreMappingService storeMappingService,
        IShoppingCartService shoppingCartService,
        ISystemClock systemClock,
        ILogger logger)
    {
        _kmOrderRepository = kmOrderRepository;
        _externalUserService = externalUserService;
        _workContext = workContext;
        _storeService = storeService;
        _storeContext = (storeContext as KmStoreContext) ?? throw new ArgumentNullException();
        _customerService = customerService;
        _addressService = addressService;
        _paymentService = paymentService;
        _orderProcessingService = orderProcessingService;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _shoppingCartService = shoppingCartService;
        _systemClock = systemClock;
        _logger = logger;
    }
    public async Task<IEnumerable<CreateOrderResponse>> CreateOrdersAsync(IEnumerable<CreateOrderRequest> requests)
    {
        if(requests == null)
            throw new ArgumentNullException(nameof(requests));
        if(!requests.Any())
            return Array.Empty<CreateOrderResponse>();  

        await _logger.InformationAsync($"Start processing orders");

        var jso = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
        var res = requests.Select(r => new CreateOrderResponse { Request = r });

        //provision users
        var userIds = requests.Select(x => x.KmUserId).Distinct().ToList();
        userIds.ThrowIfNullOrEmpty(nameof(userIds));
        var maps = await _externalUserService.ProvisionUsersAsync(userIds);

        //check if already exists
        var existKmOrderIds = (from k in _kmOrderRepository.Table
                               select k.KmOrderId).ToList();

        foreach (var r in res)
        {
            if (existKmOrderIds.Any(x => x == r.Request.KmOrderId))
            {
                r.Error = "duplicated";
                continue;
            }

            var m = maps.FirstOrDefault(x => x.KmUserId == r.Request.KmUserId);
            if (m == default || m.Customer == default)
                continue;

            var customer = m.Customer;

            await _workContext.SetCurrentCustomerAsync(customer);
            await CreateOrUpdateCustomerAddress(customer, r.Request.BillingInfo, AddressType.BillingAddress);
            await CreateOrUpdateCustomerAddress(customer, r.Request.ShippingAddress, AddressType.ShippingAddress);

            //set default store;
            if (r.Request.StoreId == 0)
                r.Request.StoreId = 1;

            var store = await _storeService.GetStoreByIdAsync(r.Request.StoreId);
            if (store == default)
                continue;

            _storeContext.SetStore(store);

            var (cart, disapproved) = await ExtractShoppingCart(r.Request.CartItems);
            r.ApprovedShoppingCartItems = cart;
            r.DisapprovedShoppingCartItems = disapproved;

            if (cart == default || !cart.Any())
            {
                r.Error = "no-cart-items";
                continue;
            }

            var processPaymentRequest = new ProcessPaymentRequest();
            _paymentService.GenerateOrderGuid(processPaymentRequest);
            processPaymentRequest.StoreId = store.Id;
            processPaymentRequest.CustomerId = customer.Id;
            processPaymentRequest.PaymentMethodSystemName = r.Request.PaymentMethod;

            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
            if (placeOrderResult.Errors.NotNullAndNotNotEmpty())
                r.Error = string.Join('\n', placeOrderResult.Errors);

            if (!r.IsError)
            {
                var kmOrder = new KmOrder
                {
                    CreatedOnUtc = _systemClock.UtcNow.DateTime,
                    Data = JsonSerializer.Serialize(r.Request, jso),
                    NopOrderId = placeOrderResult.PlacedOrder.Id,
                    NopOrder = placeOrderResult.PlacedOrder,
                    KmOrderId = r.Request.KmOrderId,
                    KmUserId = r.Request.KmUserId,
                    Status = placeOrderResult.Success ? "success" : "failed",
                    Errors = string.Join("\n\n", placeOrderResult.Errors),
                };
                await _kmOrderRepository.InsertAsync(kmOrder);
                await _logger.InformationAsync($"order added to the database. " +
                    $"{nameof(kmOrder.NopOrderId)}=\'{kmOrder.NopOrderId}\', {nameof(kmOrder.KmOrderId)}=\'{kmOrder.KmOrderId}\'");
            }
            else
            {
                await _logger.InformationAsync($"Failed to import {nameof(r.Request.KmOrderId)}=\'{r.Request.KmOrderId}\'. " +
                    $"{nameof(r.Error)}={r.Error}");
            }
        }

        return res;
    }

    internal enum AddressType
    {
        BillingAddress = 0,
        ShippingAddress = 1,
    }

    private async Task CreateOrUpdateCustomerAddress(Customer customer, Address address, AddressType addressType)
    {
        if (customer == default || address == default)
            return;

        var isBillingAddress = addressType == AddressType.BillingAddress;
        var customerAddress = isBillingAddress ?
            await _customerService.GetCustomerBillingAddressAsync(customer) :
            await _customerService.GetCustomerShippingAddressAsync(customer);

        if (customerAddress == default)
        {
            await _addressService.InsertAddressAsync(address);
            if (isBillingAddress)
            {
                customer.BillingAddressId = address.Id;
            }
            else
            {
                customer.ShippingAddressId = address.Id;
            }

            await _customerService.UpdateCustomerAsync(customer);
            await _customerService.InsertCustomerAddressAsync(customer, address);
            return;
        }

        if (!areAddressesEqual())
        {
            customerAddress.FirstName = address.FirstName;
            customerAddress.LastName = address.LastName;
            customerAddress.Email = address.Email;
            customerAddress.Company = address.Company;
            customerAddress.CountryId = address.CountryId;
            customerAddress.StateProvinceId = address.StateProvinceId;
            customerAddress.County = address.County;
            customerAddress.City = address.City;
            customerAddress.Address1 = address.Address1;
            customerAddress.Address2 = address.Address2;
            customerAddress.ZipPostalCode = address.ZipPostalCode;
            customerAddress.PhoneNumber = address.PhoneNumber;
            customerAddress.FaxNumber = address.FaxNumber;
            customerAddress.CustomAttributes = address.CustomAttributes;
            customerAddress.CreatedOnUtc = address.CreatedOnUtc;

            await _addressService.UpdateAddressAsync(customerAddress);
        }


        bool areAddressesEqual()
        {
            var bothAreNull = customerAddress == null && address == null;
            if (bothAreNull)
                return bothAreNull;


            return customerAddress.FirstName == address.FirstName &&
            customerAddress.LastName == address.LastName &&
            customerAddress.Email == address.Email &&
            customerAddress.Company == address.Company &&
            customerAddress.CountryId == address.CountryId &&
            customerAddress.StateProvinceId == address.StateProvinceId &&
            customerAddress.County == address.County &&
            customerAddress.City == address.City &&
            customerAddress.Address1 == address.Address1 &&
            customerAddress.Address2 == address.Address2 &&
            customerAddress.ZipPostalCode == address.ZipPostalCode &&
            customerAddress.PhoneNumber == address.PhoneNumber &&
            customerAddress.FaxNumber == address.FaxNumber;
        }
    }

    private async Task<(IList<ShoppingCartItem> approvedShoppingCartItems, IList<ShoppingCartItem> disapprovedShoppingCartItems)> ExtractShoppingCart(IEnumerable<ShoppingCartItem> items)
    {
        var productIds = new List<int>();

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        await clearCustomerCart();
        var cart = new List<ShoppingCartItem>();
        foreach (var sci in items)
        {
            productIds.Add(sci.ProductId);

            cart.Add(new ShoppingCartItem
            {
                ProductId = sci.ProductId,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = customer.Id,
                CustomerEnteredPrice = sci.CustomerEnteredPrice,
                Quantity = sci.Quantity,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = store.Id,
            });
        }

        List<ShoppingCartItem> approved = new(),
            disapproved = new(cart);

        var products = (await _productService.GetProductsByIdsAsync(productIds.ToArray()))
            .Where(x => !x.Deleted);

        foreach (var product in products)
        {
            if (product.LimitedToStores)
            {
                var storeMap = await _storeMappingService.GetStoreMappingsAsync(product);
                if (storeMap.All(sm => sm.StoreId != store.Id))
                {
                    await clearCustomerCart();
                    await _logger.WarningAsync($"Product with id=\'{product.Id}\' is not mapped to store with id: \'{store.Id}");

                    return (null, null);
                }
            }

            var csi = cart.First(c => c.ProductId == product.Id);
            await _shoppingCartService.AddToCartAsync(
                customer,
                product,
                ShoppingCartType.ShoppingCart,
                store.Id,
                quantity: csi.Quantity);

            approved.Add(csi);
            disapproved.Remove(csi);
        }

        return (approved, disapproved);

        async Task clearCustomerCart()
        {
            var csis = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            foreach (var csi in csis)
                await _shoppingCartService.DeleteShoppingCartItemAsync(csi);
        }
    }
}
