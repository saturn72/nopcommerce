namespace Km.Catalog.Domain;

public class KmCatalogMetadata : BaseEntity
{
    public string StoresVersion { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}