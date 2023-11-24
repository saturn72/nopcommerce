
//namespace Km.Catalog.Services;

//public class FirestoreStore<TDocument> : IStore<TDocument>
//{
//    private static readonly IReadOnlyDictionary<Type, string> Collections = new Dictionary<Type, string>
//    {
//        {typeof(ProductInfoDocument), "products"},
//    };
//    private readonly string _collectionName;
//    private readonly string _projectId;

//    public FirestoreStore(IConfiguration configuration)
//    {
//        _collectionName = Collections[typeof(TDocument)];
//        _projectId = configuration["project_id"] ?? "kedem-market";
//    }

//    private (FirestoreDb db, CollectionReference collection) GetFireStore()
//    {
//        var db = FirestoreDb.Create(_projectId);
//        var collection = db.Collection(_collectionName);
//        return (db, collection);
//    }
//    public async Task CreateOrUpdateAsync(IEnumerable<TDocument> documents)
//    {
//        var (db, collection) = GetFireStore();
//        var batch = db.StartBatch();

//        foreach (var document in documents)
//        {
//            var d = document as IDocument;
//            var docRef = d?.Id != null ? collection.Document(d.Id) : collection.Document();
//            batch.Set(docRef, document);
//        }

//        await batch.CommitAsync();
//    }

//}
