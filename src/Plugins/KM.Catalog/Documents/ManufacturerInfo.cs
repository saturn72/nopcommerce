using Google.Cloud.Firestore;

namespace KedemMarket.Catalog.Documents;

[FirestoreData]
public record ManufacturerInfo
{
    [FirestoreProperty("name")]
    public string Name { get; init; }
    [FirestoreProperty("picture")]
    public CatalogMediaInfo Picture { get; init; }
}
