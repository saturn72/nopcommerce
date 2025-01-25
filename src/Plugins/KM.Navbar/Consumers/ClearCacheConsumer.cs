using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;

namespace KM.Navbar.Consumers;
public class ClearNavbarCacheConsumer :
    IConsumer<EntityUpdatedEvent<Vendor>>,
    IConsumer<EntityDeletedEvent<Vendor>>,

    IConsumer<EntityInsertedEvent<NavbarInfo>>,
    IConsumer<EntityUpdatedEvent<NavbarInfo>>,
    IConsumer<EntityDeletedEvent<NavbarInfo>>,

    IConsumer<EntityInsertedEvent<NavbarElement>>,
    IConsumer<EntityUpdatedEvent<NavbarElement>>,
    IConsumer<EntityDeletedEvent<NavbarElement>>,

    IConsumer<EntityInsertedEvent<NavbarElementVendor>>,
    IConsumer<EntityUpdatedEvent<NavbarElementVendor>>,
    IConsumer<EntityDeletedEvent<NavbarElementVendor>>
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
    public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage) =>
        await ClearNavbarVendorCacheAsync(eventMessage.Entity?.Id);

    public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage) =>
        await ClearNavbarVendorCacheAsync(eventMessage.Entity?.Id);

    private async Task ClearNavbarVendorCacheAsync(int? vendorId)
    {
        if (vendorId <= 0)
            return;

        var nevs = await _navbarService.GetNavbarElementVendorsByVendorIdAsync(vendorId.Value);
        if (!nevs.Any())
            return;

        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);
    }
    public async Task HandleEventAsync(EntityInsertedEvent<NavbarInfo> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityUpdatedEvent<NavbarInfo> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);

    public async Task HandleEventAsync(EntityDeletedEvent<NavbarInfo> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);

    public async Task HandleEventAsync(EntityInsertedEvent<NavbarElement> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityUpdatedEvent<NavbarElement> eventMessage) =>
       await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityDeletedEvent<NavbarElement> eventMessage) =>
       await ClearNavbarCacheAsync(eventMessage.Entity);

    public async Task HandleEventAsync(EntityInsertedEvent<NavbarElementVendor> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityUpdatedEvent<NavbarElementVendor> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityDeletedEvent<NavbarElementVendor> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);

    private async Task ClearNavbarCacheAsync(BaseEntity entity)
    {
        if (entity == null)
            return;

        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);
    }
}
