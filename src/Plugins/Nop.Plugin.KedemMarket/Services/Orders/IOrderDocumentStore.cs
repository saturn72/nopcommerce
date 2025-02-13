using KedemMarket.Documents;
using KedemMarket.Services.Documents;

namespace KedemMarket.Services.Orders;

public interface IOrderDocumentStore : IDocumentStore<FirestoreCartDocument>
{
    Task<IEnumerable<FirestoreCartDocument>> GetNewOrderPageAsync(
        int pageSize = 100,
        int offset = 0);
}
