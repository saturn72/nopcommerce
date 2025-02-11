using KedemMarket.Common;
using KedemMarket.Common.Models.Cart;
using KedemMarket.Common.Services.Media;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services.Attributes;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace KedemMarket.Api.Factories;

public class ShoppingCartFactory : ShoppingCartModelFactory, IShoppingCartFactory
{
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly MediaConvertor _mediaConvertor;

    public ShoppingCartFactory(
        AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICheckoutAttributeFormatter checkoutAttributeFormatter,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IDiscountService discountService,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IOrderProcessingService orderProcessingService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IShortTermCacheManager shortTermCacheManager,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        ITaxService taxService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings,
        ShoppingCartSettings shoppingCartSettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings,
        IProductAttributeParser productAttributeParser,
        MediaConvertor mediaConvertor) : base(addressSettings, captchaSettings, catalogSettings, commonSettings, customerSettings, addressModelFactory, addressService, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeFormatter, countryService, currencyService, customerService, dateTimeHelper, discountService, downloadService, genericAttributeService, giftCardService, httpContextAccessor, localizationService, orderProcessingService, orderTotalCalculationService, paymentPluginManager, paymentService, permissionService, pictureService, priceFormatter, productAttributeFormatter, productService, shippingService, shoppingCartService, shortTermCacheManager, stateProvinceService, staticCacheManager, storeContext, storeMappingService, taxService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, orderSettings, rewardPointsSettings, shippingSettings, shoppingCartSettings, taxSettings, vendorSettings)
    {
        _productAttributeParser = productAttributeParser;
        _mediaConvertor = mediaConvertor;
    }

    public async Task<CreateOrderRequest> ToCreateOrderRequest(CartTransactionApiModel model, List<string> errors)
    {
        var items = await ToShoppingCartItems(model.Items, errors);
        return new CreateOrderRequest
        {
            CartItems = items,
            PaymentMethod = model.PaymentMethod.ToSystemPaymentMethod(),
            StorePickup = model.StorePickup,
            BillingInfo = model.BillingInfo.ToAddress(),
            UpdateBillingInfo = model.UpdateBillingInfo,
            ShippingInfo = model.ShippingInfo.ToAddress(),
            UpdateShippingInfo = model.UpdateShippingInfo,
        };
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

    public async Task<CheckoutCartApiModel> PrepareCheckoutCartApiModelAsync(IList<ShoppingCartItem> cart)
    {
        var modelItems = new List<CheckoutCartItemApiModel>();

        foreach (var sci in cart)
        {
            var productAttribute = (await _productAttributeParser.ParseProductAttributeValuesAsync(sci.AttributesXml)).FirstOrDefault();
            var i = await PrepareShoppingCartItemModelAsync(cart, sci);
            modelItems.Add(await ToShoppingCartItemApiModel(sci, i, productAttribute));
        }
        return new()
        {
            Items = modelItems,
        };
    }

    private async Task<CheckoutCartItemApiModel> ToShoppingCartItemApiModel(ShoppingCartItem sci, ShoppingCartModel.ShoppingCartItemModel model, ProductAttributeValue productAttribute)
    {
        var product = await _productService.GetProductByIdAsync(sci.ProductId);
        var sciPicture = await _pictureService.GetProductPictureAsync(product, sci.AttributesXml);
        var thumbnail = sciPicture != default ?
            await _mediaConvertor.GetDownloadLinkAsync(sciPicture.Id, KmConsts.MediaTypes.Image)
            : default;

        return new()
        {
            Id = model.Id,
            AllowItemEditing = model.AllowItemEditing,
            DisableRemoval = model.DisableRemoval,
            VendorName = model.VendorName,
            Picture = model.Picture,
            ProductId = model.ProductId,
            ProductName = model.ProductName,
            ProductSeName = model.ProductSeName,
            UnitPrice = model.UnitPrice,
            UnitPriceValue = model.UnitPriceValue,
            SubTotal = model.SubTotal,
            SubTotalValue = model.SubTotalValue,
            Discount = model.Discount,
            DiscountValue = model.DiscountValue,
            MaximumDiscountedQty = model.MaximumDiscountedQty,
            Quantity = model.Quantity,
            AllowedQuantities = model.AllowedQuantities,
            AttributeInfo = model.AttributeInfo,
            RecurringInfo = model.RecurringInfo,
            RentalInfo = model.RentalInfo,
            Sku = model.Sku,
            Thumbnail = thumbnail,
            Warnings = model.Warnings,
            VariantId = productAttribute?.ProductAttributeMappingId ?? 0,
            OptionId = productAttribute?.Id ?? 0,
        };
    }
}
