using KM.Api.Models.Directory;
using Microsoft.AspNetCore.Http;

namespace KM.Api.Factories;

public class ShoppingCartFactory : IShoppingCartFactory
{
    private readonly IStoreContext _storeContext;
    private readonly IProductService _productService;
    private readonly IProductAttributeParser _productAttributeParser;

    public ShoppingCartFactory(IStoreContext storeContext,
        IProductService productService,
        IProductAttributeParser productAttributeParser)
    {
        _storeContext = storeContext;
        _productService = productService;
        _productAttributeParser = productAttributeParser;
    }

    public async Task<CreateOrderRequest> ToCreateOrderRequest(CartTransactionApiModel model, List<string> errors)
    {
        var items = await ToShoppingCartItems(model.Items, errors);
        return new CreateOrderRequest
        {
            CartItems = items,
            PaymentMethod = model.PaymentMethod.ToSystemPaymentMethod(),
            StorePickup = model.StorePickup,
            BillingInfo = toAddress(model.BillingInfo),
            UpdateBillingInfo = model.BillingInfo.UpdateUserInfo,
            ShippingInfo = toAddress(model.ShippingInfo),
            UpdateShippingInfo = model.ShippingInfo.UpdateUserInfo,
        };

        static Address toAddress(ContactInfoModel ci)
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
    }

    public async Task<IList<ShoppingCartItem>> ToShoppingCartItems(IEnumerable<ShoppingCartItemApiModel> items, List<string> errors)
    {
        var productIds = items.Select(x => x.ProductId).ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);
        var store = await _storeContext.GetCurrentStoreAsync();
        var res = new List<ShoppingCartItem>();
        foreach (var i in items)
        {
            var product = products.FirstOrDefault(p => p.Id == i.ProductId);
            var attributesXml = default(string);
            if (i.VariantId != 0)
            {
                var form = new FormCollection(new() { { $"{NopCatalogDefaults.ProductAttributePrefix}{i.VariantId}", i.OptionId.ToString() }, });
                attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);
            }

            res.Add(new()
            {
                ProductId = i.ProductId,
                AttributesXml = attributesXml,
                Quantity = i.Quantity,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = store.Id,
            });
        }

        return res;
    }
}
