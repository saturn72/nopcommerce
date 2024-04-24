using Google.Cloud.Firestore;

namespace Km.Catalog.Documents;

[FirestoreData]
public record ProductOption
{
    public string Caption { get; init; }
}