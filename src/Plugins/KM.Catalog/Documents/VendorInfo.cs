namespace Km.Catalog.Documents;

[FirestoreData]
public record VendorInfo : IDocument
{
    [FirestoreProperty]
    public string id { get; init; }
    [FirestoreProperty]
    public string name { get; init; }
    [FirestoreProperty]
    public CatalogMediaInfo logo { get; init; }
}
