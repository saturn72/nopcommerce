using Km.Catalog.Documents;

namespace Km.Catalog.EventConsumers;

public class KmStoresSnapshotInsertedEventConsumer :
    IConsumer<EntityInsertedEvent<KmStoresSnapshot>>
{
    private readonly IHubContext<CatalogHub> _hub;
    private readonly IStorageManager _storageManager;

    public KmStoresSnapshotInsertedEventConsumer(
        IHubContext<CatalogHub> hub,
        IStorageManager storageManager)
    {
        _hub = hub;
        _storageManager = storageManager;
    }

    public Task HandleEventAsync(EntityInsertedEvent<KmStoresSnapshot> eventMessage)
    {
        var e = eventMessage.Entity;
        var json = JsonSerializer.Deserialize<object>(e.Json);
        var o = new
        {
            version = e.Version,
            createdOnUtc = e.CreatedOnUtc,
            stores = json
        };

        return Task.WhenAll(new[]
        {
            _hub.Clients.All.SendAsync("updated"),
            _storageManager.UploadAsync("catalog/index.json", "application/json", o),
        });
    }
}
