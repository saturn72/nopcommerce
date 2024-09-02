using Microsoft.Extensions.Options;

namespace KM.Api.Services.Media;

public class GcpStorageManager : IStorageManager
{
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<GcpOptions> _options;
    private readonly FirebaseAdapter _firebaseAdapter;
    public GcpStorageManager(
        IOptionsMonitor<GcpOptions> options,
        FirebaseAdapter fbAdapter,
        ILogger logger)
    {
        _options = options;
        _firebaseAdapter = fbAdapter;
        _logger = logger;
    }

    public Task DeleteAsync(string path)
    {
        var p = HandlePath(path);
        return _firebaseAdapter.Storage.DeleteObjectAsync(_options.CurrentValue.BucketName, path);
    }

    public async Task UploadAsync(string path, string contentType, byte[] bytes)
    {
        var p = HandlePath(path);
        await _logger.InformationAsync($"Start uploading object to path: {p}");

        using var stream = new MemoryStream(bytes);
        var res = await _firebaseAdapter.Storage.UploadObjectAsync(_options.CurrentValue.BucketName, p, contentType, stream);
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
