namespace Km.Catalog.Documents;

public record StoreInfo
{
    public string Id { get; init; }
    public string Name { get; init; }
    public CatalogMediaInfo LogoThumb { get; init; }
    public CatalogMediaInfo LogoPicture { get; init; }
    public IEnumerable<ProductInfoDocument> Products { get; init; }
    public IEnumerable<string> StructuredData { get; init; }
    public IEnumerable<VendorInfo> Vendors { get; init; }
}
