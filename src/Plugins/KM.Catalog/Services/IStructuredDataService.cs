

namespace Km.Catalog.Services;

public interface IStructuredDataService
{
    Task<object?> GenerateStoreStructuredDataAsync(Store store);
    Task<object?> GenerateProductStructuredDataAsync(Product product);
}
