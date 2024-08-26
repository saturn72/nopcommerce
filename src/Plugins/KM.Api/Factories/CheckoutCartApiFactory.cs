using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
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

namespace KM.Api.Factories;

public class CheckoutCartApiFactory : ShoppingCartModelFactory, ICheckoutCartApiFactory
{
    private readonly IProductAttributeParser _productAttributeParser;

    public CheckoutCartApiFactory(
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
        IStoreContext storeContext
        , IStoreMappingService storeMappingService,
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
        IProductAttributeParser productAttributeParser) : base(addressSettings, captchaSettings, catalogSettings, commonSettings, customerSettings, addressModelFactory, addressService, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeFormatter, countryService, currencyService, customerService, dateTimeHelper, discountService, downloadService, genericAttributeService, giftCardService, httpContextAccessor, localizationService, orderProcessingService, orderTotalCalculationService, paymentPluginManager, paymentService, permissionService, pictureService, priceFormatter, productAttributeFormatter, productService, shippingService, shoppingCartService, shortTermCacheManager, stateProvinceService, staticCacheManager, storeContext, storeMappingService, taxService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, orderSettings, rewardPointsSettings, shippingSettings, shoppingCartSettings, taxSettings, vendorSettings)
    {
        _productAttributeParser = productAttributeParser;
    }

    public async Task<CheckoutCartApiModel> PrepareCheckoutCartApiModelAsync(IList<ShoppingCartItem> cart)
    {
        var modelItems = new List<CheckoutCartItemApiModel>();

        foreach (var sci in cart)
        {
            var productAttribute = (await _productAttributeParser.ParseProductAttributeValuesAsync(sci.AttributesXml)).FirstOrDefault();
            var i = await base.PrepareShoppingCartItemModelAsync(cart, sci);
            modelItems.Add(ToShoppingCartItemApiModel(i, productAttribute));
        }
        return new()
        {
            Items = modelItems,
        };
    }

    private CheckoutCartItemApiModel ToShoppingCartItemApiModel(ShoppingCartModel.ShoppingCartItemModel source, ProductAttributeValue productAttribute)
    {
        return new()
        {
            Id = source.Id,
            Sku = source.Sku,
            VendorName = source.VendorName,
            Picture = source.Picture,
            ProductId = source.ProductId,
            ProductName = source.ProductName,
            ProductSeName = source.ProductSeName,
            UnitPrice = source.UnitPrice,
            UnitPriceValue = source.UnitPriceValue,
            SubTotal = source.SubTotal,
            SubTotalValue = source.SubTotalValue,
            Discount = source.Discount,
            DiscountValue = source.DiscountValue,
            MaximumDiscountedQty = source.MaximumDiscountedQty,
            Quantity = source.Quantity,
            AllowedQuantities = source.AllowedQuantities,
            AttributeInfo = source.AttributeInfo,
            RecurringInfo = source.RecurringInfo,
            RentalInfo = source.RentalInfo,
            AllowItemEditing = source.AllowItemEditing,
            DisableRemoval = source.DisableRemoval,
            Warnings = source.Warnings,
            VariantId = productAttribute?.ProductAttributeMappingId ?? 0,
            OptionId = productAttribute?.Id ?? 0,
        };
    }
}
