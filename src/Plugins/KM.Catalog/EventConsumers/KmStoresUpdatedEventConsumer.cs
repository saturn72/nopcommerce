namespace Km.Catalog.EventConsumers;

public class KmStoresUpdatedEventConsumer :
    IConsumer<EntityUpdatedEvent<StoreSnapshot>>,
    IConsumer<EntityUpdatedEvent<ProductsSnapshot>>

{
    private readonly IHubContext<CatalogHub> _hub;
    private readonly IStorageManager _storageManager;
    private readonly IRepository<CatalogMetadata> _catalogMetadataRepository;

    public KmStoresUpdatedEventConsumer(
        IHubContext<CatalogHub> hub,
        IRepository<CatalogMetadata> catalogMetadataRepository,
        IStorageManager storageManager)
    {
        _hub = hub;
        _storageManager = storageManager;
        _catalogMetadataRepository = catalogMetadataRepository;
    }

    public Task HandleEventAsync(EntityUpdatedEvent<StoreSnapshot> eventMessage)
    {
        return Task.WhenAll(new[]
        {
            _hub.Clients.All.SendAsync("stores-updated"),
            UpdateCatalogMetadataAsync(md => md.storesVersion = eventMessage.Entity.Version);
        ;
    }

    public Task HandleEventAsync(EntityUpdatedEvent<ProductsSnapshot> eventMessage)
    {
        return Task.WhenAll(new[]
        {
            _hub.Clients.All.SendAsync("catalog-updated"),
            UpdateCatalogMetadataAsync(md => md.productInfos = eventMessage.Entity.Version),
        });
    }

    private async Task UpdateCatalogMetadataAsync(Action<CatalogMetadata> config)
    {
        var md = (from a in _catalogMetadataRepository.Table
                  select a).Take(1)?.FirstOrDefault();

        if (md == null)
        {
            md = new CatalogMetadata();
            config(md);
            await _catalogMetadataRepository.InsertAsync(md);
        }
        else
        {
            config(md);
            await _catalogMetadataRepository.UpdateAsync(md);
        }
        _storageManager.UploadAsync("catalog/metadata.json", "application/json", md);
    }
}
