using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Nop.Plugin.Misc.KM.Catalog.Domain;

namespace Nop.Plugin.Misc.KM.Catalog.Services
{
    public class GcpStorageManager : IStorageManager
    {
        private readonly IOptionsMonitor<GcpOptions> _options;
        private readonly string[] _scopes = new[]
        {
           "https://www.googleapis.com/auth/cloud-platform",
           "https://www.googleapis.com/auth/firebase",
       };
        private StorageClient _storageClient;

        public GcpStorageManager(IOptionsMonitor<GcpOptions> options)
        {
            _options = options;
        }

        public async Task<IStorageManager.StoredFileInfo> UploadAsync(string path, string contentType, Stream stream)
        {
            var storage = await GetStorage();
            var p = HandlePath(path);
            var res = await storage.UploadObjectAsync(_options.CurrentValue.BucketName, p, contentType, stream);
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
            while(path.StartsWith('/'))
                path = path.Substring(1);
            path = path.Replace("  ", " ").Replace(' ', '-').ToLower();
            return path;
        }
    }
}
