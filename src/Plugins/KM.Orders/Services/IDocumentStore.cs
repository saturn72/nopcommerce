namespace KM.Orders.Services;

public interface IDocumentStore<TDocument> where TDocument : IDocument
{
    Task<IEnumerable<TDocument>> GetPageAsync(
        string fromIndex = null,
        string afterIndex = null,
        int pageSize = 100,
        int offset = 0);

    Task UpdateAsync(string id, Dictionary<string, object> updater);
}
