namespace Km.Catalog.Documents;

[FirestoreData]
public record CatalogMediaInfo
{
    [FirestoreProperty]
    public int displayOrder { get; init; }
    [FirestoreProperty]
    public string alt { get; init; }
    [FirestoreProperty]
    public string title { get; init; }
    [FirestoreProperty]
    public string type { get; set; }
    [FirestoreProperty]
    public string uri { get; init; }
}
