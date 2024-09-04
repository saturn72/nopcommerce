using KM.Api.Models.Media;
using KM.Api.Services.Media;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Models.Media;

namespace KM.Api.Factories;
public sealed class MediaConvertor
{
    private readonly IStorageManager _storageManager;

    public MediaConvertor(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    public async Task<object> ToMediaItemAsync(Picture picture, int displayOrder = 0)
    {
        if (picture == null)
            return null;

        var pictureService = EngineContext.Current.Resolve<IPictureService>();
        var mediaSettings = EngineContext.Current.Resolve<MediaSettings>();
        //var (url, _) = await EngineContext.Current.Resolve<IPictureService>().GetPictureUrlAsync(picture);

        (var cartImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.CartThumbPictureSize);
        (var detailImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.ProductDetailsPictureSize);
        (var fullImage, _) = await pictureService.GetPictureUrlAsync(picture);
        (var thumbImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.ProductThumbPictureSize);
        (var variantImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.ImageSquarePictureSize);

        return new
        {
            alt = picture.AltAttribute,
            cartImage,
            fullImage,
            index = displayOrder,
            mimeType = picture.MimeType,
            seoFilename = picture.SeoFilename,
            thumbImage,
            title = picture.TitleAttribute,
            type = "image",
            detailImage,
        };
    }

    public object ToMediaItem(Video video, int displayOrder = 0)
    {
        return new
        {
            type = "video",
            index = displayOrder,
            url = video.VideoUrl,
        };
    }

    public async Task<GalleryItemModel> ToGalleryItemModel(PictureModel pictureModel, int index)
    {
        string fiUrl = default, tiUrl = default;
        if (pictureModel.Id > 0)
        {
            var fp = _storageManager.BuildWebpPath(KmApiConsts.MediaTypes.Image, pictureModel.Id);
            fiUrl = await _storageManager.GetDownloadLink(fp);
            var tp = _storageManager.BuildWebpPath(KmApiConsts.MediaTypes.Thumbnail, pictureModel.Id);
            tiUrl = await _storageManager.GetDownloadLink(tp);
        }

        return new()
        {
            Alt = pictureModel.AlternateText,
            FullImage = fiUrl,
            Index = index,
            ThumbImage = tiUrl,
            Title = pictureModel.Title,
            Type = "image"
        };
    }

    public GalleryItemModel ToGalleryItemModel(VideoModel videoModel, int index)
    {
        return new()
        {
            Index = index,
            Url = videoModel.VideoUrl,
            Width = videoModel.Width,
            Height = videoModel.Height,
            Type = "video"
        };
    }
}
