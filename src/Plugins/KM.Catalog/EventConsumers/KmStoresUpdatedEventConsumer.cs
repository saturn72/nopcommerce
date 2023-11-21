namespace Km.Catalog.EventConsumers;

public class KmStoresUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<StoreSnapshot>>
{
    private readonly IHubContext<CatalogHub> _hub;
    private readonly IStaticCacheManager _cacheManager;
    public KmStoresUpdatedEventConsumer(IHubContext<CatalogHub> hub, IStaticCacheManager cacheManager)
    {
        _hub = hub;
        _cacheManager = cacheManager;
    }
    public Task HandleEventAsync(EntityUpdatedEvent<StoreSnapshot> eventMessage)
    {
        return Task.WhenAll(new[] {
            _hub.Clients.All.SendAsync("stores-updated"),
            _cacheManager.RemoveAsync(MetadataController.CacheKey)
        });
    }
}
