using System.IO;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.KM.Catalog.Services
{
    public interface IStorageManager
    {
        Task<StoredFileInfo> UploadAsync(string path, string mediaType, Stream stream);
        public record StoredFileInfo
        {
            public string Storage { get; init; }
            public string StorageIdentifier { get; init; }
        }
    }
}
