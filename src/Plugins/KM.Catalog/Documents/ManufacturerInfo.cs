using Google.Cloud.Firestore;

namespace Km.Catalog.Documents;

[FirestoreData]
public record ManufacturerInfo
{
    [FirestoreProperty("name")]
    public string Name { get; init; }
    [FirestoreProperty("picture")]
    public CatalogMediaInfo Picture { get; init; }
}
