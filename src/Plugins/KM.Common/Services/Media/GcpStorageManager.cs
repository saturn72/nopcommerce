using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Nop.Core.Caching;
using Nop.Services.Logging;

namespace KM.Common.Services.Media;
public class GcpStorageManager : IStorageManager
{
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<GcpOptions> _options;
    private readonly IStaticCacheManager _staticCache;
    private readonly string[] _scopes = new[]
       {
       "https://www.googleapis.com/auth/cloud-platform",
       "https://www.googleapis.com/auth/firebase",
    };
    private readonly StorageClient _storageClient;
    private static readonly int DefaultCachingTime = (int)TimeSpan.FromDays(7).Subtract(TimeSpan.FromMinutes(30)).TotalMinutes;
    private CacheKey GetCacheKey(string path) => new CacheKey(path)
    {
        CacheTime = DefaultCachingTime
    };
    public GcpStorageManager(
        IOptionsMonitor<GcpOptions> options,
        IStaticCacheManager staticCache,
        ILogger logger)
    {
        _options = options;
        _staticCache = staticCache;
        _logger = logger;

        var cred = GoogleCredential.GetApplicationDefault();
        _ = cred.CreateScoped(_scopes)
      .UnderlyingCredential;

        _storageClient = StorageClient.Create();
    }

    public Task DeleteAsync(string path)
    {
        var p = HandlePath(path);
        return _storageClient.DeleteObjectAsync(_options.CurrentValue.BucketName, path);
    }

    public async Task UploadAsync(string path, string contentType, byte[] bytes)
    {
        var p = HandlePath(path);
        await _logger.InformationAsync($"Start uploading object to path: {p}");

        using var stream = new MemoryStream(bytes);
        var res = await _storageClient.UploadObjectAsync(_options.CurrentValue.BucketName, p, contentType, stream);
        await _logger.InformationAsync($"Finish uploading to bucket. Success =  {(res.Id.IsNullOrEmpty() ? "false" : "true")}");
        await Task.Yield();
        _ = GetOrCreateDownloadLink(p);
    }


    protected virtual async Task<string> GetOrCreateDownloadLink(string path)
    {
        var urlSigner = _storageClient.CreateUrlSigner();
        var url = await urlSigner.SignAsync(_options.CurrentValue.BucketName, path, TimeSpan.FromDays(7), HttpMethod.Get);
        var key = GetCacheKey(path);
        await _staticCache.SetAsync(key, url);
        return url;
    }

    public async Task<string?> GetDownloadLink(string path)
    {
        var p = HandlePath(path);
        var key = GetCacheKey(p);
        var value = await _staticCache.GetAsync(key, () => GetOrCreateDownloadLink(p));
        return value.HasValue() ? value : default;
    }

    private string HandlePath(string path)
    {
        //remove heading slashes
        while (path.StartsWith('/'))
            path = path.Substring(1);
        path = path.Replace("  ", " ").Replace(' ', '-').ToLower();
        return path;
    }

    public string BuildWebpPath(string type, int pictureId) => $"/{type}/{pictureId}.webp";
}
