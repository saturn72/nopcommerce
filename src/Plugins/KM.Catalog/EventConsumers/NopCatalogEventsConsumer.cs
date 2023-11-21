
namespace Km.Catalog.EventConsumers;

public class NopCatalogEventsConsumer :
    IConsumer<EntityInsertedEvent<Vendor>>,
    IConsumer<EntityUpdatedEvent<Vendor>>,
    IConsumer<EntityDeletedEvent<Vendor>>,

    IConsumer<EntityInsertedEvent<Category>>,
    IConsumer<EntityUpdatedEvent<Category>>,
    IConsumer<EntityDeletedEvent<Category>>,

    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityUpdatedEvent<Product>>,
    IConsumer<EntityDeletedEvent<Product>>,

    IConsumer<EntityInsertedEvent<ProductPicture>>,
    IConsumer<EntityUpdatedEvent<ProductPicture>>,
    IConsumer<EntityDeletedEvent<ProductPicture>>,

    IConsumer<EntityInsertedEvent<Warehouse>>,
    IConsumer<EntityUpdatedEvent<Warehouse>>,
    IConsumer<EntityDeletedEvent<Warehouse>>,

    IConsumer<EntityInsertedEvent<Manufacturer>>,
    IConsumer<EntityUpdatedEvent<Manufacturer>>,
    IConsumer<EntityDeletedEvent<Manufacturer>>
{
    private static Task TriggerUpdateStorageTask()
    {
        UpdateCatalogTask.EnqueueCatalogUpdateRequest();
        return Task.CompletedTask;
    }
    public Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityInsertedEvent<Warehouse> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<Warehouse> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<Warehouse> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage) => TriggerUpdateStorageTask();
}
