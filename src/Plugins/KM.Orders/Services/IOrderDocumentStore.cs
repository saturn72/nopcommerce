namespace Km.Orders.Services;

public interface IOrderDocumentStore : IDocumentStore<FirestoreCartDocument>
{
    Task<IEnumerable<FirestoreCartDocument>> GetNewOrderPageAsync(
        int pageSize = 100,
        int offset = 0);
}
