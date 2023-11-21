namespace Km.Catalog.EventConsumers;

public class NopStoreEventsConsumer :
    IConsumer<EntityInsertedEvent<Store>>,
    IConsumer<EntityUpdatedEvent<Store>>,
    IConsumer<EntityDeletedEvent<Store>>
{
    private static Task TriggerUpdateStorageTask()
    {
        UpdateStoresTask.EnqueueStoresUpdateRequest();
        return Task.CompletedTask;
    }
    public Task HandleEventAsync(EntityInsertedEvent<Store> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityUpdatedEvent<Store> eventMessage) => TriggerUpdateStorageTask();
    public Task HandleEventAsync(EntityDeletedEvent<Store> eventMessage) => TriggerUpdateStorageTask();
}
