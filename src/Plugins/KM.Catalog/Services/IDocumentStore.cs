namespace KM.Catalog.Services;

public interface IDocumentStore
{
    Task<object> InsertAsync(string collectionName, object document);
}
