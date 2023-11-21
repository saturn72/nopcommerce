namespace Km.Catalog.Documents;

[FirestoreData]
public record StoresSnapshotInfo : IDocument
{
    [FirestoreProperty]
    public string id { get; init; }
    [FirestoreProperty]
    public long version { get; init; }

    [FirestoreProperty]
    public IEnumerable<StoreInfo> stores { get; init; }
}
