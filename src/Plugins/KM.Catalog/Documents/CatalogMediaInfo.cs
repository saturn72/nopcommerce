using Google.Cloud.Firestore;

namespace Km.Catalog.Documents;

[FirestoreData]
public record CatalogMediaInfo
{
    [FirestoreProperty("displayOrder")]
    public int DisplayOrder { get; init; }

    [FirestoreProperty("alt")]
    public string Alt { get; init; }

    [FirestoreProperty("title")]
    public string Title { get; init; }
    
    [FirestoreProperty("type")]
    public string Type { get; set; }
    
    [FirestoreProperty("uri")]
    public string Uri { get; init; }
}
