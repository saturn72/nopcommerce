namespace Km.Catalog.Documents;

[FirestoreData]
public record ManufacturerInfo
{
    [FirestoreProperty]
    public string name { get; init; }
    [FirestoreProperty]
    public CatalogMediaInfo picture { get; init; }
}
