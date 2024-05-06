namespace Km.Api.Services;
public class OrderDocumentStore : FirebaseDocumentStore<FirestoreCartDocument>, IOrderDocumentStore
{
    public OrderDocumentStore(IConfiguration configuration) : base(configuration, "orders")
    {
    }

    public async Task<IEnumerable<FirestoreCartDocument>> GetNewOrderPageAsync(int pageSize = 100, int offset = 0)
    {
        var colRef = GetFireStoreCollection().WhereEqualTo("status", "submitted");

        var q = colRef.Offset(offset)
                .Limit(pageSize);

        var snapshot = await q.GetSnapshotAsync();

        var res = new List<FirestoreCartDocument>();

        foreach (var sd in snapshot.Documents)
        {
            if (!sd.Exists)
                continue;

            var t = sd.ConvertTo<FirestoreCartDocument>();
            t.id = sd.Id;
            res.Add(t);
        }
        return res;
    }
}
