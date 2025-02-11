namespace KedemMarket.Common.Services.Media;
public interface IStorageManager
{
    Task DeleteAsync(string path);
    Task UploadAsync(string path, string contentType, byte[] bytes);
    Task<string> CreateDownloadLinkAsync(string webpPath);
    string GetWebpPath(string mediaType, int pictureId);
}
