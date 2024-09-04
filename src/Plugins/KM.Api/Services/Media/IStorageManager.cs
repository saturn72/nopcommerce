namespace KM.Api.Services.Media;
public interface IStorageManager
{
    Task DeleteAsync(string path);
    Task UploadAsync(string path, string contentType, byte[] bytes);
    Task<string> GetDownloadLink(string path);
    string BuildWebpPath(string type, int pictureId);
}
