
using Google.Cloud.Firestore;

namespace Km.Catalog.Services;

public class FirestoreDocumentStore : IDocumentStore
{   private readonly string _projectId;
    private FirestoreDb _db;

    public FirestoreDocumentStore(IConfiguration configuration)
    {
        _projectId = configuration["project_id"] ?? "kedem-market";
    }
    public async Task<object> InsertAsync(string collectionName, object document)
    {
        _db ??= FirestoreDb.Create(_projectId);
        var collection = _db.Collection(collectionName);
        var docRef = await collection.AddAsync(document);
        var snapshot = await docRef.GetSnapshotAsync();
        return snapshot.ConvertTo<object>();
    }
}
