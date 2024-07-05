using KM.Catalog.Documents;

namespace KM.Catalog.Services;

public interface IMediaItemInfoService
{
    Task<KmMediaItemInfo> GetOrCreateMediaItemInfoAsync(string type, Picture picture, int displayOrder);
}
