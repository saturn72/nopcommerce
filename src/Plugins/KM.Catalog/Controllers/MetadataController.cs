
namespace Km.Catalog.Controllers;

[ApiController]
[Area("api")]
[Route("api/metadata")]
public class MetadataController : ControllerBase
{
    internal static CacheKey CacheKey = new CacheKey("catalog-metadata");
    private readonly IStaticCacheManager _cacheManager;
    private readonly IRepository<StoreSnapshot> _storeSnapshotRepository;

    public MetadataController(
        IStaticCacheManager cacheManager,
        IRepository<StoreSnapshot> storeSnapshotRepository)
    {
        _cacheManager = cacheManager;
        _storeSnapshotRepository = storeSnapshotRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalogVersionsAsync()
    {
        var data = await _cacheManager.GetAsync(CacheKey, BuildMetadata);
        return Ok(data);

    }
    private async Task<object> BuildMetadata()
    {
        var lastStores = await _storeSnapshotRepository.GetLastAsync();
        return new
        {
            storesVersion = lastStores.Version,
        };
    }
}
