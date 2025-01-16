using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;

namespace KM.Navbar.Consumers;
public class ClearNavbarCacheConsumer :
    IConsumer<EntityUpdatedEvent<Vendor>>,
    IConsumer<EntityUpdatedEvent<NavbarInfo>>,
    IConsumer<EntityUpdatedEvent<NavbarElement>>
//    ,
//IConsumer<EntityUpdatedEvent<Category>>,
//IConsumer<EntityUpdatedEvent<Product>>,
{
    private readonly INavbarService _navbarService;
    private IStaticCacheManager _staticCacheManager;

    public ClearNavbarCacheConsumer(
        INavbarService navbarService,
        IStaticCacheManager staticCacheManager)
    {
        _navbarService = navbarService;
        _staticCacheManager = staticCacheManager;
    }
    public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
    {
        var vendorId = eventMessage.Entity?.Id;
        if (vendorId <= 0)
            return;

        var nevs = await _navbarService.GetNavbarElementVendorsByVendorIdAsync(vendorId.Value);
        if (!nevs.Any())
            return;

        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<NavbarInfo> eventMessage)
    {
        if (eventMessage.Entity == null)
            return;

        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<NavbarElement> eventMessage)
    {
        if (eventMessage.Entity == null)
            return;

        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);
    }
}
