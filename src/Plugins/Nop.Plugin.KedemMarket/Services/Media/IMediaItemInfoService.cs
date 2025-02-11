using KedemMarket.Models.Media;
using Nop.Core.Domain.Media;

namespace KedemMarket.Services.Media;

public interface IMediaItemInfoService
{
    Task<KmMediaItemInfo> GetOrCreateMediaItemInfoAsync(string type, Picture picture, int displayOrder);
}
