using Google.Cloud.Firestore;

namespace Km.Catalog.Documents;

[FirestoreData]
public record StoreInfo
{
    [FirestoreProperty("id")]
    public string Id { get; init; }

    [FirestoreProperty("name")]
    public string Name { get; init; }
    
    [FirestoreProperty("logoThumb")]
    public CatalogMediaInfo LogoThumb { get; init; }
    
    [FirestoreProperty("logoPicture")]
    public CatalogMediaInfo LogoPicture { get; init; }
    
    [FirestoreProperty("products")]
    public IEnumerable<ProductInfoDocument> Products { get; init; }
    
    [FirestoreProperty("structuredData")]
    public IEnumerable<string> StructuredData { get; init; }
    
    [FirestoreProperty("vendors")]
    public IEnumerable<VendorInfo> Vendors { get; init; }
}
