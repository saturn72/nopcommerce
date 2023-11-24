namespace Km.Catalog.Domain;

public class KmStoresSnapshot : BaseEntity
{
    public string Json { get; set; }
    public uint Version { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
