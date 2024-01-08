
using KM.Catalog.Models;

namespace KM.Catalog.Factories;

public interface IKmCatalogModelFactory
{
    public Task PrepareConfigurationModelAsync(ConfigurationModel model);
}
