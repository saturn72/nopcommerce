namespace Km.Catalog.Documents;

public record CatalogMediaInfo
{
    public int DisplayOrder { get; init; }
    public string Alt { get; init; }
    public string Title { get; init; }
    public string Type { get; set; }
    public string Uri { get; init; }
}
