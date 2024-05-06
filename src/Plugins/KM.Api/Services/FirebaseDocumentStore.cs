
namespace Km.Api.Services
{
    public class FirebaseDocumentStore<TDocument> : IDocumentStore<TDocument> where TDocument : IDocument
    {
        private readonly string _collectionName;
        private readonly string _projectId;

        public FirebaseDocumentStore(IConfiguration configuration, string collectionName)
        {
            _collectionName = collectionName;
            _projectId = configuration["project_id"] ?? "kedem-market";
        }

        protected virtual CollectionReference GetFireStoreCollection()
        {
            var db = FirestoreDb.Create(_projectId);
            return db.Collection(_collectionName);
        }

        public async Task<IEnumerable<TDocument>> GetPageAsync(
            string fromIndex = null,
            string afterIndex = null,
            int pageSize = 100,
            int offset = 0)
        {
            var collection = GetFireStoreCollection();
            var c = collection.OrderBy(FieldPath.DocumentId);

            if (fromIndex != null)
                c = c.StartAt(fromIndex);
            else
            {

                if (afterIndex != null)
                    c = c.StartAfter(afterIndex);
            }

            var q = c.Offset(offset)
                    .Limit(pageSize);

            var snapshot = await q.GetSnapshotAsync();

            var res = new List<TDocument>();

            foreach (var sd in snapshot.Documents)
            {
                if (!sd.Exists)
                    continue;

                var t = sd.ConvertTo<TDocument>();
                t.id = sd.Id;
                res.Add(t);
            }
            return res;
        }

        public async Task UpdateAsync(string id, Dictionary<string, object> updates)
        {
            var collection = GetFireStoreCollection();

            var doc = collection.Document(id);
            var snapshot = await doc.GetSnapshotAsync();
            if (!snapshot.Exists)
                return;

            await doc.UpdateAsync(updates);
        }
    }
}
