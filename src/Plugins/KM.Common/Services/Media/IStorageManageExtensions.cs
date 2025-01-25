namespace KM.Common.Services.Media;

public static class IStorageManageExtensions
{
    public static async Task<string> CreateDownloadLink(this IStorageManager storageManager, string mediaType, int pictureId)
    {
        var webpPath = storageManager.GetWebpPath(mediaType, pictureId);
        return await storageManager.CreateDownloadLinkAsync(webpPath);
    }
}
