namespace Km.Catalog.Documents;

public record VendorInfo
{
    public string Id { get; init; }
    public string Name { get; init; }
    public CatalogMediaInfo Logo { get; init; }
}
