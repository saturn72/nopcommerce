using Nop.Core.Domain.Media;
using Nop.Services.Media;

namespace KM.Api.Controllers;
public class MediaPreperar
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
            variantImage,
        };
        //        return new
        //        {
        //            alt = picture.AltAttribute,
        //            type = "image",
        //            index = displayOrder,
        //            url = url,
        //            mimeType = picture.MimeType,
        //#warning - optimize for thumb
        //            thumb = picture.VirtualPath,
        //            seoFilename = picture.SeoFilename,
        //            title = picture.TitleAttribute,
        //        };
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
}
