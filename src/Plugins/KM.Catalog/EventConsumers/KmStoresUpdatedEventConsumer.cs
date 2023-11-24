namespace Km.Catalog.EventConsumers;

public class KmStoresUpdatedEventConsumer :
    IConsumer<EntityUpdatedEvent<KmStoresSnapshot>>
{
    private readonly IHubContext<CatalogHub> _hub;
    private readonly IStorageManager _storageManager;
    private readonly IRepository<KmCatalogMetadata> _catalogMetadataRepository;

    public KmStoresUpdatedEventConsumer(
        IHubContext<CatalogHub> hub,
        IRepository<KmCatalogMetadata> catalogMetadataRepository,
        IStorageManager storageManager)
    {
        _hub = hub;
        _storageManager = storageManager;
        _catalogMetadataRepository = catalogMetadataRepository;
    }

    public Task HandleEventAsync(EntityUpdatedEvent<KmStoresSnapshot> eventMessage)
    {
        return Task.WhenAll(new[]
        {
            _hub.Clients.All.SendAsync("updated"),
            UpdateCatalogMetadataAsync(md => md.StoresVersion = eventMessage.Entity.Version.ToString()),
        });
    }

    private async Task UpdateCatalogMetadataAsync(Action<KmCatalogMetadata> config)
    {
        var newMd = new KmCatalogMetadata();
        var md = await _catalogMetadataRepository.GetLatestAsync();

        if (md != null)
        {
            newMd.StoresVersion = md.StoresVersion;
        }

        config(newMd);
        await _catalogMetadataRepository.InsertAsync(md);
        await _storageManager.UploadAsync("catalog/metadata.json", "application/json", md);
    }
}
