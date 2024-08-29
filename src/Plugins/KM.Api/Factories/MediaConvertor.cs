using KM.Api.Models.Media;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Models.Media;

namespace KM.Api.Factories;
public sealed class MediaConvertor
{
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

    public GalleryItemModel ToGalleryItemModel(PictureModel pictureModel, int index)
    {
        return new()
        {
            Alt = pictureModel.AlternateText,
            FullImage = pictureModel.FullSizeImageUrl,
            Index = index,
            ThumbImage = pictureModel.ThumbImageUrl,
            Title = pictureModel.Title,
            Type = "image"
        };
    }

    public GalleryItemModel ToGalleryItemModel(VideoModel videoModel, int index)
    {
        return new()
        {
            Url = videoModel.VideoUrl,
            Width = videoModel.Width,
            Height = videoModel.Height,
            Type = "video"
        };
    }
}
