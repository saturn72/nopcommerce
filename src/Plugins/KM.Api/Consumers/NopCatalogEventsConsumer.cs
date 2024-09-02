
namespace KM.Api.Consumer;

public class NopCatalogEventsConsumer :

    IConsumer<EntityInsertedEvent<Category>>,
    IConsumer<EntityUpdatedEvent<Category>>,
    IConsumer<EntityDeletedEvent<Category>>,

    IConsumer<EntityInsertedEvent<CrossSellProduct>>,
    IConsumer<EntityUpdatedEvent<CrossSellProduct>>,
    IConsumer<EntityDeletedEvent<CrossSellProduct>>,

    IConsumer<EntityInsertedEvent<DiscountManufacturerMapping>>,
    IConsumer<EntityUpdatedEvent<DiscountManufacturerMapping>>,
    IConsumer<EntityDeletedEvent<DiscountManufacturerMapping>>,

    IConsumer<EntityInsertedEvent<DiscountCategoryMapping>>,
    IConsumer<EntityUpdatedEvent<DiscountCategoryMapping>>,
    IConsumer<EntityDeletedEvent<DiscountCategoryMapping>>,

    IConsumer<EntityInsertedEvent<DiscountProductMapping>>,
    IConsumer<EntityUpdatedEvent<DiscountProductMapping>>,
    IConsumer<EntityDeletedEvent<DiscountProductMapping>>,

    IConsumer<EntityInsertedEvent<LocalizedProperty>>,
    IConsumer<EntityUpdatedEvent<LocalizedProperty>>,
    IConsumer<EntityDeletedEvent<LocalizedProperty>>,

    IConsumer<EntityInsertedEvent<Manufacturer>>,
    IConsumer<EntityUpdatedEvent<Manufacturer>>,
    IConsumer<EntityDeletedEvent<Manufacturer>>,

    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityUpdatedEvent<Product>>,
    IConsumer<EntityDeletedEvent<Product>>,

    IConsumer<EntityInsertedEvent<ProductAttributeCombination>>,
    IConsumer<EntityUpdatedEvent<ProductAttributeCombination>>,
    IConsumer<EntityDeletedEvent<ProductAttributeCombination>>,

    IConsumer<EntityInsertedEvent<ProductAttributeMapping>>,
    IConsumer<EntityUpdatedEvent<ProductAttributeMapping>>,
    IConsumer<EntityDeletedEvent<ProductAttributeMapping>>,

    IConsumer<EntityInsertedEvent<ProductCategory>>,
    IConsumer<EntityUpdatedEvent<ProductCategory>>,
    IConsumer<EntityDeletedEvent<ProductCategory>>,

    IConsumer<EntityInsertedEvent<ProductManufacturer>>,
    IConsumer<EntityUpdatedEvent<ProductManufacturer>>,
    IConsumer<EntityDeletedEvent<ProductManufacturer>>,

    IConsumer<EntityInsertedEvent<ProductPicture>>,
    IConsumer<EntityUpdatedEvent<ProductPicture>>,
    IConsumer<EntityDeletedEvent<ProductPicture>>,

    IConsumer<EntityInsertedEvent<ProductProductTagMapping>>,
    IConsumer<EntityUpdatedEvent<ProductProductTagMapping>>,
    IConsumer<EntityDeletedEvent<ProductProductTagMapping>>,

    IConsumer<EntityInsertedEvent<ProductReview>>,
    IConsumer<EntityUpdatedEvent<ProductReview>>,
    IConsumer<EntityDeletedEvent<ProductReview>>,

    IConsumer<EntityInsertedEvent<ProductReviewHelpfulness>>,
    IConsumer<EntityUpdatedEvent<ProductReviewHelpfulness>>,
    IConsumer<EntityDeletedEvent<ProductReviewHelpfulness>>,

    IConsumer<EntityInsertedEvent<ProductSpecificationAttribute>>,
    IConsumer<EntityUpdatedEvent<ProductSpecificationAttribute>>,
    IConsumer<EntityDeletedEvent<ProductSpecificationAttribute>>,

    IConsumer<EntityInsertedEvent<ProductTag>>,
    IConsumer<EntityUpdatedEvent<ProductTag>>,
    IConsumer<EntityDeletedEvent<ProductTag>>,

    IConsumer<EntityInsertedEvent<ProductVideo>>,
    IConsumer<EntityUpdatedEvent<ProductVideo>>,
    IConsumer<EntityDeletedEvent<ProductVideo>>,

    IConsumer<EntityInsertedEvent<ProductWarehouseInventory>>,
    IConsumer<EntityUpdatedEvent<ProductWarehouseInventory>>,
    IConsumer<EntityDeletedEvent<ProductWarehouseInventory>>,

    IConsumer<EntityInsertedEvent<RelatedProduct>>,
    IConsumer<EntityUpdatedEvent<RelatedProduct>>,
    IConsumer<EntityDeletedEvent<RelatedProduct>>,

    IConsumer<EntityInsertedEvent<Shipment>>,
    IConsumer<EntityUpdatedEvent<Shipment>>,
    IConsumer<EntityDeletedEvent<Shipment>>,

    IConsumer<EntityInsertedEvent<StockQuantityHistory>>,
    IConsumer<EntityUpdatedEvent<StockQuantityHistory>>,
    IConsumer<EntityDeletedEvent<StockQuantityHistory>>,

    IConsumer<EntityInsertedEvent<Store>>,
    IConsumer<EntityUpdatedEvent<Store>>,
    IConsumer<EntityDeletedEvent<Store>>,

    IConsumer<EntityInsertedEvent<TierPrice>>,
    IConsumer<EntityUpdatedEvent<TierPrice>>,
    IConsumer<EntityDeletedEvent<TierPrice>>,

    IConsumer<EntityInsertedEvent<Vendor>>,
    IConsumer<EntityUpdatedEvent<Vendor>>,
    IConsumer<EntityDeletedEvent<Vendor>>,

    IConsumer<EntityInsertedEvent<Warehouse>>,
    IConsumer<EntityUpdatedEvent<Warehouse>>,
    IConsumer<EntityDeletedEvent<Warehouse>>
{
    private readonly IPriorityQueue _queue;
    private readonly IHubContext<CatalogHub> _hub;
    private readonly JsonSerializerOptions _jso = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

    public NopCatalogEventsConsumer(
            IPriorityQueue queue,
            IHubContext<CatalogHub> hub)
    {
        _queue = queue;
        _hub = hub;
    }

    private Task TriggerUpdateStorageTask<TEntity>(TEntity entity)
    {
        _queue.Enqueue("catalog", entity, Handler);
        return Task.CompletedTask;
    }

    private Task Handler<TEntity>(TEntity entity)
    {
        return _hub.Clients.All.SendAsync("catalog-updated", entity.GetType().Name, JsonSerializer.Serialize(entity, _jso));
    }

    public Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<CrossSellProduct> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<CrossSellProduct> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<CrossSellProduct> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<DiscountManufacturerMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<DiscountManufacturerMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<DiscountManufacturerMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<DiscountCategoryMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<DiscountCategoryMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<DiscountCategoryMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<DiscountProductMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<DiscountProductMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<DiscountProductMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<LocalizedProperty> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<LocalizedProperty> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<LocalizedProperty> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductAttributeCombination> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeCombination> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductAttributeCombination> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductAttributeMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductAttributeMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductCategory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductCategory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductCategory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductManufacturer> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductManufacturer> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductManufacturer> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductProductTagMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductProductTagMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductProductTagMapping> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductReview> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductReview> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductReview> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductReviewHelpfulness> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductReviewHelpfulness> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductReviewHelpfulness> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductSpecificationAttribute> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductTag> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductTag> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductTag> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductVideo> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductVideo> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductVideo> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<ProductWarehouseInventory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<ProductWarehouseInventory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<ProductWarehouseInventory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<RelatedProduct> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<RelatedProduct> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<RelatedProduct> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<Shipment> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Shipment> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Shipment> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<StockQuantityHistory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<StockQuantityHistory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<StockQuantityHistory> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<Store> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Store> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Store> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<TierPrice> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<TierPrice> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<TierPrice> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);

    public Task HandleEventAsync(EntityInsertedEvent<Warehouse> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityUpdatedEvent<Warehouse> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
    public Task HandleEventAsync(EntityDeletedEvent<Warehouse> eventMessage) => TriggerUpdateStorageTask(eventMessage.Entity);
}
