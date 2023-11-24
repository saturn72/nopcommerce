namespace Km.Catalog.Documents;

public record StoresSnapshotInfo
{
    public string Id { get; init; }
    public long Version { get; init; }

    public IEnumerable<StoreInfo> Stores { get; init; }
}
