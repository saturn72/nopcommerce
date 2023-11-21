namespace Km.Catalog.Services;

public interface IStore<TDocument>
{
    Task CreateOrUpdateAsync(IEnumerable<TDocument> documents);
}
