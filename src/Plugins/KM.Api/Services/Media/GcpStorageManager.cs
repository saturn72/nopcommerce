using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace KM.Api.Services.Media;
public class GcpStorageManager : IStorageManager
{
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<GcpOptions> _options;

    private readonly string[] _scopes = new[]
       {
       "https://www.googleapis.com/auth/cloud-platform",
       "https://www.googleapis.com/auth/firebase",
    };
    private readonly StorageClient _storageClient;

    public GcpStorageManager(
        IOptionsMonitor<GcpOptions> options,
        ILogger logger)
    {
        _options = options;
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
