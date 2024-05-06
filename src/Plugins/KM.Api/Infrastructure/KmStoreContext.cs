
namespace Km.Api.Infrastructure
{
    public class KmStoreContext : IStoreContext
    {
        private Store _store;

        public Task<int> GetActiveStoreScopeConfigurationAsync() => Task.FromResult(_store?.Id ?? 0);

        public Store GetCurrentStore() => _store;

        public Task<Store> GetCurrentStoreAsync() => Task.FromResult(_store);
        public void SetStore(Store store) => _store = store;
    }
}
