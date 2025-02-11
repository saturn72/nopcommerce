using KedemMarket.Admin.Domain;
using KedemMarket.Services.Navbar;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;

namespace KedemMarket.Consumers;
public class ClearNavbarCacheConsumer :
    IConsumer<EntityUpdatedEvent<Vendor>>,
    IConsumer<EntityDeletedEvent<Vendor>>,

    IConsumer<EntityDeletedEvent<VendorAttributeValue>>,
    IConsumer<EntityInsertedEvent<VendorAttributeValue>>,
    IConsumer<EntityUpdatedEvent<VendorAttributeValue>>,

    IConsumer<EntityInsertedEvent<NavbarInfo>>,
    IConsumer<EntityUpdatedEvent<NavbarInfo>>,
    IConsumer<EntityDeletedEvent<NavbarInfo>>,

    IConsumer<EntityInsertedEvent<NavbarElement>>,
    IConsumer<EntityUpdatedEvent<NavbarElement>>,
    IConsumer<EntityDeletedEvent<NavbarElement>>,

    IConsumer<EntityInsertedEvent<NavbarElementVendor>>,
    IConsumer<EntityUpdatedEvent<NavbarElementVendor>>,
    IConsumer<EntityDeletedEvent<NavbarElementVendor>>,
    
    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityUpdatedEvent<Product>>,
    IConsumer<EntityDeletedEvent<Product>>

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

    public async Task HandleEventAsync(EntityDeletedEvent<VendorAttributeValue> eventMessage) =>
        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);

    public async Task HandleEventAsync(EntityInsertedEvent<VendorAttributeValue> eventMessage) =>
        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);

    public async Task HandleEventAsync(EntityUpdatedEvent<VendorAttributeValue> eventMessage) =>
        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);

    private async Task ClearNavbarVendorCacheAsync(int? vendorId)
    {
        if (vendorId <= 0)
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

    public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);
    public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage) =>
        await ClearNavbarCacheAsync(eventMessage.Entity);

    private async Task ClearNavbarCacheAsync(BaseEntity entity)
    {
        if (entity == null)
            return;

        await _staticCacheManager.RemoveByPrefixAsync(NavbarCacheSettings.NAVBAR_CACHE_KEY);
    }
}
