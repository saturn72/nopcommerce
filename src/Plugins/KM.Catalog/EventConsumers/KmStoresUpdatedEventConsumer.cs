namespace Km.Catalog.EventConsumers;

public class KmStoresUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<StoreSnapshot>>
{
    private readonly IHubContext<CatalogHub> _hub;
    private readonly IStorageManager _storageManager;

    public KmStoresUpdatedEventConsumer(
        IHubContext<CatalogHub> hub,
        IStorageManager storageManager)
    {
        _hub = hub;
        _storageManager = storageManager;
    }
    public Task HandleEventAsync(EntityUpdatedEvent<StoreSnapshot> eventMessage)
    {
        return Task.WhenAll(new[] {
            _hub.Clients.All.SendAsync("stores-updated"),
            UpdateCatalogMetadataAsync(eventMessage.Entity),
        });
    }

    private Task UpdateCatalogMetadataAsync(StoreSnapshot snapshot)
    {
        var j = new
        {
            storesVersion = snapshot.Version,
        };

        return _storageManager.UploadAsync("catalog/metadata.json", "application/json", j);
    }
}
