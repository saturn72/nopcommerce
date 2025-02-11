using KedemMarket.Api.Models.Store;
using Nop.Services.Vendors;

namespace KedemMarket.Api.Controllers;

[Route("api/marketplace")]
public class MarketplaceController : KmApiControllerBase
{
    private readonly IStoreContext _storeContext;
    private readonly IVendorService _vendorService;
    private readonly IVendorApiModelFactory _vendorApiModelFactory;
    private readonly IDirectoryFactory _directoryFactory;

    public MarketplaceController(
        IStoreContext storeContext,
        IVendorService vendorService,
        IVendorApiModelFactory vendorApiModelFactory,
        IDirectoryFactory directoryFactory)
    {
        _storeContext = storeContext;
        _vendorService = vendorService;
        _vendorApiModelFactory = vendorApiModelFactory;
        _directoryFactory = directoryFactory;
        _directoryFactory = directoryFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoreInfoByStoreIdAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        if (store == default)
            return BadRequest();

        var phone = _directoryFactory.ProcessPhoneNumber(store.CompanyPhoneNumber);
        var data = new StoreInfoApiModel
        {
            StoreName = store.Name,
            DisplayName = store.DefaultTitle,
            Phone = phone,
            Url = store.Url,
            SocialLinks = new Dictionary<string, string>
            {
                { KmApiConsts.SocialLinkNames.Facebook , "https://www.facebook.com/KedemMarket.co.il" },
                { KmApiConsts.SocialLinkNames.Instagram , "https://www.instagram.com/kedemmarket.co.il/"},
                { KmApiConsts.SocialLinkNames.Linktr , "https://linktr.ee/kedemmarket" },
            }
        };
        return ToJsonResult(data);
    }
}
