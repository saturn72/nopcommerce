using KedemMarket.Catalog.Documents;
using Nop.Core.Domain.Media;

namespace KedemMarket.Common.Services.Media;

public interface IMediaItemInfoService
{
    Task<KmMediaItemInfo> GetOrCreateMediaItemInfoAsync(string type, Picture picture, int displayOrder);
}
