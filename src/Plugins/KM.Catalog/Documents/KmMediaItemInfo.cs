namespace KM.Catalog.Documents;

public partial class KmMediaItemInfo
{
    public string Uri { get; set; }
    public byte[] BinaryData { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public string Type { get; set; }
    public string Storage { get; set; }
    public string StorageIdentifier { get; set; }
}
