

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
        var o = new
        {
            version = e.Version,
            stores = e.Json
        };

        await _documentStore.InsertAsync("catalog", o);
        await _hub.Clients.All.SendAsync("updated");
    }
}
