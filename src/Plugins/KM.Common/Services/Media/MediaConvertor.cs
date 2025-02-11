using KedemMarket.Common.Models.Media;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Web.Models.Media;

namespace KedemMarket.Common.Services.Media;
public sealed class MediaConvertor
{
    private readonly IStorageManager _storageManager;
    private readonly IStaticCacheManager _staticCache;

    public MediaConvertor(
        IStorageManager storageManager,
        IStaticCacheManager staticCache)
    {
        _storageManager = storageManager;
        _staticCache = staticCache;
    }

    public async Task<string> GetDownloadLinkAsync(int pictureId, string mediaType)
    {
        var path = _storageManager.GetWebpPath(mediaType, pictureId);
        var key = new CacheKey(path)
        {
            CacheTime = (int)TimeSpan.FromDays(7).Subtract(TimeSpan.FromMinutes(30)).TotalMinutes,
        };

        return await _staticCache.GetAsync(key, async () => await _storageManager.CreateDownloadLinkAsync(path));
    }
    public async Task<GalleryItemModel> ToGalleryItemModel(Picture picture, int index)
    {
        picture.ThrowArgumentNullException(nameof(picture));

        return new()
        {
            Alt = picture.AltAttribute,
            FullImage = await GetDownloadLinkAsync(picture.Id, KmConsts.MediaTypes.Image),
            Index = index,
            ThumbImage = await GetDownloadLinkAsync(picture.Id, KmConsts.MediaTypes.Thumbnail),
            Title = picture.TitleAttribute,
            Type = "image"
        };
    }

    public async Task<GalleryItemModel> ToGalleryItemModel(PictureModel picture, int index)
    {
        return new()
        {
            Alt = picture.AlternateText,
            FullImage = await GetDownloadLinkAsync(picture.Id, KmConsts.MediaTypes.Image),
            Index = index,
            ThumbImage = await GetDownloadLinkAsync(picture.Id, KmConsts.MediaTypes.Thumbnail),
            Title = picture.Title,
            Type = "image"
        };
    }


    public GalleryItemModel ToGalleryItemModel(Video video, int index)
    {
        return new()
        {
            Index = index,
            Type = "video",
            Url = video.VideoUrl,
        };
    }
}
