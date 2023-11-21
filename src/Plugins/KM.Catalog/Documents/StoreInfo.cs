namespace Km.Catalog.Documents;

[FirestoreData]
public record StoreInfo : IDocument
{
    [FirestoreProperty]
    public string id { get; init; }
    [FirestoreProperty]
    public string name { get; init; }
    [FirestoreProperty]
    public KmMediaItemInfo logoThumb { get; init; }
    [FirestoreProperty]
    public KmMediaItemInfo logoPicture { get; internal set; }
}
