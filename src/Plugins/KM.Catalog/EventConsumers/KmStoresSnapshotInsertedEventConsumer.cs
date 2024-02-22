

namespace Km.Catalog.EventConsumers;

public class KmStoresSnapshotInsertedEventConsumer :
    IConsumer<EntityInsertedEvent<KmStoresSnapshot>>
{
    private readonly IHubContext<CatalogHub> _hub;
    private readonly IDocumentStore _documentStore;

    public KmStoresSnapshotInsertedEventConsumer(
        IHubContext<CatalogHub> hub,
        IDocumentStore documentStore)
    {
        _hub = hub;
        _documentStore = documentStore;
    }

    public async Task HandleEventAsync(EntityInsertedEvent<KmStoresSnapshot> eventMessage)
    {
        var e = eventMessage.Entity;

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        var stores = JsonSerializer.Deserialize<List<StoreInfo>>(e.Data, options);

        var o = new
        {
            version = e.Version,
            stores
        };

        await _documentStore.InsertAsync("catalog", o);
        await _hub.Clients.All.SendAsync("updated");
    }
}
