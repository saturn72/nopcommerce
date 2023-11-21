namespace Km.Catalog.Documents;

[FirestoreData]
public record VendorStoreInfo : IDocument
{
    [FirestoreProperty]
    public string id { get; init; }
    [FirestoreProperty]
    public string name { get; init; }
}
