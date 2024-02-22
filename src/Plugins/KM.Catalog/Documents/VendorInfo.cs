using Google.Cloud.Firestore;

namespace Km.Catalog.Documents;

[FirestoreData]
public record VendorInfo
{
    [FirestoreProperty("id")]
    public string Id { get; init; }

    [FirestoreProperty("name")]
    public string Name { get; init; }

    [FirestoreProperty("logo")]
    public CatalogMediaInfo Logo { get; init; }
}
