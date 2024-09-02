
namespace KM.Catalog.Services;

public class GcpStorageManager : IStorageManager
{
    private readonly IOptionsMonitor<GcpOptions> _options;
    private readonly ILogger _logger;
    private readonly string[] _scopes = new[]
    {
       "https://www.googleapis.com/auth/cloud-platform",
       "https://www.googleapis.com/auth/firebase",
    };
    private StorageClient _storageClient;

    public GcpStorageManager(
        IOptionsMonitor<GcpOptions> options,
        ILogger logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<IStorageManager.StoredFileInfo> UploadAsync(string path, string contentType, Stream stream)
    {
        var storage = await GetStorage();
        var p = HandlePath(path);
        await _logger.InformationAsync($"Start uploading object. Bucket name: {_options.CurrentValue.BucketName}");

        var res = await storage.UploadObjectAsync(_options.CurrentValue.BucketName, p, contentType, stream);

        await _logger.InformationAsync($"Finish uploading to bucket. Success =  {(res.Id.IsNullOrEmpty() ? "false" : "true")}");

        return new()
        {
            StorageIdentifier = res.Id,
            Storage = "firebase"
        };
    }

    private async Task<StorageClient> GetStorage()
    {
        if (_storageClient == null)
        {
            var gc = await GoogleCredential.GetApplicationDefaultAsync();
            _ = gc.CreateScoped(_scopes)
            .UnderlyingCredential;

            _storageClient = await StorageClient.CreateAsync();
        }
        return _storageClient;
    }

    private string HandlePath(string path)
    {
        //remove heading slashes
        while (path.StartsWith('/'))
            path = path.Substring(1);
        path = path.Replace("  ", " ").Replace(' ', '-').ToLower();
        return path;
    }
}
