using KM.Catalog.Documents;
using Nop.Core.Domain.Media;

namespace KM.Common.Services.Media;

public interface IMediaItemInfoService
{
    Task<KmMediaItemInfo> GetOrCreateMediaItemInfoAsync(string type, Picture picture, int displayOrder);
}
