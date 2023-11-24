namespace Nop.Data;
public static class RepositoryExtensions
{
    public static async Task<KmStoresSnapshot> GetLatestAsync(this IRepository<KmStoresSnapshot> repository)
    {
        var l = await repository.GetAllAsync(q => q.OrderByDescending(x => x.CreatedOnUtc).Take(1));
        return l?.FirstOrDefault();
    }

    public static async Task<KmCatalogMetadata> GetLatestAsync(this IRepository<KmCatalogMetadata> repository)
    {
        var l = await repository.GetAllAsync(q => q.OrderByDescending(x => x.CreatedOnUtc).Take(1));
        return l?.FirstOrDefault();
    }
}
