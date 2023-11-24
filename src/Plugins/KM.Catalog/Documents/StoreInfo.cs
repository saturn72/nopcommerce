namespace Km.Catalog.Documents;

public record StoreInfo 
{
    public string Id { get; init; }
    public string Name { get; init; }
    public KmMediaItemInfo LogoThumb { get; init; }
    public KmMediaItemInfo LogoPicture { get; init; }
    public IEnumerable<ProductInfoDocument> Products { get; init; }
    public IEnumerable<VendorInfo> Vendors { get; init; }
}
