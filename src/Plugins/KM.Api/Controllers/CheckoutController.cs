using KM.Api.Models.Directory;

namespace KM.Api.Controllers;

[Route("api/checkout")]
public class CheckoutController : KmApiControllerBase
{
    private readonly IKmOrderService _kmOrderService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;

    public CheckoutController(
        IKmOrderService kmOrderService,
        IShoppingCartService shoppingCartService,
        IWorkContext workContext,
        IStoreContext storeContext)
    {
        _kmOrderService = kmOrderService;
        _shoppingCartService = shoppingCartService;
        _workContext = workContext;
        _storeContext = storeContext;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] CartTransactionApiModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var kmUSerId = Request.Headers[KmApiConsts.USER_ID];
        var store = await _storeContext.GetCurrentStoreAsync();
        var co = new CreateOrderRequest
        {
            KmUserId = kmUSerId,
            StoreId = store.Id,
            CartItems = toCartItems(),
            PaymentMethod = model.PaymentMethod.ToSystemPaymentMethod(),
            StorePickup = model.StorePickup,
            BillingInfo = toAddress(model.BillingInfo),
            UpdateBillingInfo = model.BillingInfo.UpdateUserInfo,
            ShippingInfo = toAddress(model.ShippingInfo),
            UpdateShippingInfo = model.ShippingInfo.UpdateUserInfo,
        };

        var created = await _kmOrderService.CreateOrdersAsync(new[] { co });
        var c = created.First();
        if (c.IsError)
            return BadRequest(new { error = c.Error });

        await clearCustomerCart();
        return Accepted();

        Address toAddress(ContactInfoModel ci)
        {
            var names = ci.Fullname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var lastName = ci.Fullname[names[0].Length..].Trim();
            return new Address
            {
                FirstName = names[0],
                LastName = lastName,
                Email = ci.Email,
                PhoneNumber = ci.Phone,
                Address1 = ci.Address.Street,
                City = ci.Address.City,
                ZipPostalCode = ci.Address.PostalCode,
                //CountryId == need to add countryId
            };
        }
        async Task clearCustomerCart()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _shoppingCartService.ClearShoppingCartAsync(customer, store.Id);
        }

        IEnumerable<ShoppingCartItem> toCartItems() =>
            model.Items.Select(s => new ShoppingCartItem { ProductId = s.ProductId, Quantity = s.Quantity, CustomerEnteredPrice = s.CustomerEnteredPrice });
    }
}
