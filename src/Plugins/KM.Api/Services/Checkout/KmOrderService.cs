
using Microsoft.AspNetCore.Http;

namespace KM.Api.Services.Checkout;
public class KmOrderService : IKmOrderService
{
    private readonly IRepository<KmOrder> _kmOrderRepository;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly ICustomerService _customerService;
    private readonly IAddressService _addressService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ISystemClock _systemClock;
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly ILogger _logger;

    public KmOrderService(
        IRepository<KmOrder> kmOrderRepository,
        IWorkContext workContext,
        IStoreContext storeContext,
        ICustomerService customerService,
        IAddressService addressService,
        IPaymentService paymentService,
        IOrderProcessingService orderProcessingService,
        IProductService productService,
        IStoreMappingService storeMappingService,
        IShoppingCartService shoppingCartService,
        ISystemClock systemClock,
        IHttpContextAccessor httpAccessor,
        ILogger logger
        )
    {
        _kmOrderRepository = kmOrderRepository;
        _workContext = workContext;
        _storeContext = storeContext;
        _customerService = customerService;
        _addressService = addressService;
        _paymentService = paymentService;
        _orderProcessingService = orderProcessingService;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _shoppingCartService = shoppingCartService;
        _systemClock = systemClock;
        _httpAccessor = httpAccessor;
        _logger = logger;
    }
    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _logger.InformationAsync($"Start processing orders");
        var res = new CreateOrderResponse { Request = request };

        var cart = request.CartItems;
        //var (cart, disapproved) = await ExtractShoppingCart(request.CartItems);
        //res.DisapprovedShoppingCartItems = disapproved;
        res.ApprovedShoppingCartItems = cart;
        res.DisapprovedShoppingCartItems = Array.Empty<ShoppingCartItem>();

        if (cart == default || !cart.Any())
        {
            res.Error = "no-cart-items";
            return res;
        }
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (request.UpdateBillingInfo || customer.BillingAddressId == 0)
            await CreateOrUpdateCustomerAddress(customer, request.BillingInfo, AddressType.BillingAddress);
        if (request.UpdateShippingInfo || customer.ShippingAddressId == 0)
            await CreateOrUpdateCustomerAddress(customer, request.ShippingInfo, AddressType.ShippingAddress);

        var store = await _storeContext.GetCurrentStoreAsync();
        var processPaymentRequest = new ProcessPaymentRequest();
        await _paymentService.GenerateOrderGuidAsync(processPaymentRequest);
        processPaymentRequest.StoreId = store.Id;
        processPaymentRequest.CustomerId = customer.Id;
        processPaymentRequest.PaymentMethodSystemName = request.PaymentMethod;
        
        var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
        if (placeOrderResult.Errors.NotNullAndNotNotEmpty())
        {
            res.Error = string.Join('\n', placeOrderResult.Errors);
        }
        else
        {
            var jso = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
            var kmOrder = new KmOrder
            {
                CreatedOnUtc = _systemClock.UtcNow.DateTime,
                Data = JsonSerializer.Serialize(request, jso),
                NopOrderId = placeOrderResult.PlacedOrder.Id,
                NopOrder = placeOrderResult.PlacedOrder,
                KmUserId = _httpAccessor.HttpContext.Request.Headers[KmApiConsts.USER_ID],
                Status = placeOrderResult.Success ? "success" : "failed",
                Errors = string.Join("\n\n", placeOrderResult.Errors),
            };
            await _kmOrderRepository.InsertAsync(kmOrder);
            await _logger.InformationAsync($"order added to the database. {nameof(kmOrder.NopOrderId)}=\'{kmOrder.NopOrderId}\'");
            await _shoppingCartService.ClearShoppingCartAsync(customer, store.Id);
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
        /*
         * this code might be redundant
         */
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
                AttributesXml = sci.AttributesXml,
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
