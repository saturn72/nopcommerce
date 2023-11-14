
using System.Threading.Tasks;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.KM.Catalog.Domain;

namespace Nop.Plugin.Misc.KM.Catalog.Services
{
    public interface IMediaItemInfoService
    {
        Task<MediaItemInfo> GetOrCreateMediaItemInfoAsync(string type, Picture picture, int displayOrder);
    }
}
