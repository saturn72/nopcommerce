using Nop.Core.Domain.Directory;

namespace KedemMarket.Catalog.Services;

public interface IStructuredDataService
{
    Task<object> GenerateProductStructuredDataAsync(Product product, Currency currency);
    Task<object?> GenerateStoreStructuredDataAsync(Store store);
}
