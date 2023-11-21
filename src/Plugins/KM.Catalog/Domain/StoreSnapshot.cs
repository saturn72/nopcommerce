namespace Km.Catalog.Domain;

public class StoreSnapshot : BaseEntity
{
    public string Json { get; set; }
    public uint Version { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
