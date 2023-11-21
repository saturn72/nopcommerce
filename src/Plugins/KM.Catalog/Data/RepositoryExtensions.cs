namespace Nop.Data;
public static class RepositoryExtensions
{
    public static async Task<StoreSnapshot> GetLastAsync(this IRepository<StoreSnapshot> repository)
    {
        var l = await repository.GetAllAsync(q => q.OrderByDescending(x => x.CreatedOnUtc).Take(1));
        return l?.FirstOrDefault();
    }
}
