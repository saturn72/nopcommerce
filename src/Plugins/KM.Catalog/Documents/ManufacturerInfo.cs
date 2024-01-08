namespace KM.Catalog.Documents;

public record ManufacturerInfo
{
    public string Name { get; init; }
    public CatalogMediaInfo Picture { get; init; }
}
