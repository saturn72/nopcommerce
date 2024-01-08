
namespace KM.Catalog.EventConsumers
{
    public class ProductLifecycleConsumer :
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly KmCatalogSettings _kmCatalogSettings;

        public ProductLifecycleConsumer(
            IRepository<Product> productRepository,
            KmCatalogSettings kmCatalogSettings)
        {
            _productRepository = productRepository;
            _kmCatalogSettings = kmCatalogSettings;
        }
        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage) =>
            await UpdateProductVendor(eventMessage.Entity);
        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage) =>
            await UpdateProductVendor(eventMessage.Entity);

        private async Task UpdateProductVendor(Product product)
        {
            if (product.VendorId != 0)
                return;

            product.VendorId = _kmCatalogSettings.DefaultVendorId;
            await _productRepository.UpdateAsync(product, false);
        }
    }
}
