using KM.Common.Models.Media;
using Nop.Core.Domain.Media;
using Nop.Web.Models.Media;

namespace KM.Common.Services.Media;
public sealed class MediaConvertor
{
    private readonly IStorageManager _storageManager;

    public MediaConvertor(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    public Task<string> ToThumbnail(int pictureId)
    {
        var tp = _storageManager.BuildWebpPath(KmConsts.MediaTypes.Thumbnail, pictureId);
        return _storageManager.GetDownloadLink(tp);
    }

    public Task<string> ToImage(int pictureId)
    {
        var fp = _storageManager.BuildWebpPath(KmConsts.MediaTypes.Image, pictureId);
        return _storageManager.GetDownloadLink(fp);
    }
    public async Task<GalleryItemModel> ToGalleryItemModel(Picture picture, int index)
    {
        picture.ThrowArgumentNullException(nameof(picture));

        return new()
        {
            Alt = picture.AltAttribute,
            FullImage = await ToImage(picture.Id),
            Index = index,
            ThumbImage = await ToThumbnail(picture.Id),
            Title = picture.TitleAttribute,
            Type = "image"
        };
    }

    public async Task<GalleryItemModel> ToGalleryItemModel(PictureModel picture, int index)
    {
        return new()
        {
            Alt = picture.AlternateText,
            FullImage = await ToImage(picture.Id),
            Index = index,
            ThumbImage = await ToThumbnail(picture.Id),
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
