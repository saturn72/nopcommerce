using KM.Api.Models.Media;
using KM.Api.Services.Media;

namespace KM.Api.Factories;
public sealed class MediaConvertor
{
    private readonly IStorageManager _storageManager;

    public MediaConvertor(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    //public async Task<object> ToMediaItemAsync(Picture picture, int displayOrder)
    //{
    //    if (picture == null)
    //        return null;

    //    var pictureService = EngineContext.Current.Resolve<IPictureService>();
    //    var mediaSettings = EngineContext.Current.Resolve<MediaSettings>();
    //    //var (url, _) = await EngineContext.Current.Resolve<IPictureService>().GetPictureUrlAsync(picture);

    //    (var cartImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.CartThumbPictureSize);
    //    (var detailImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.ProductDetailsPictureSize);
    //    (var fullImage, _) = await pictureService.GetPictureUrlAsync(picture);
    //    (var thumbImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.ProductThumbPictureSize);
    //    (var variantImage, _) = await pictureService.GetPictureUrlAsync(picture, mediaSettings.ImageSquarePictureSize);

    //    return new
    //    {
    //        alt = picture.AltAttribute,
    //        cartImage,
    //        fullImage,
    //        index = displayOrder,
    //        mimeType = picture.MimeType,
    //        seoFilename = picture.SeoFilename,
    //        thumbImage,
    //        title = picture.TitleAttribute,
    //        type = "image",
    //        detailImage,
    //    };
    //}


    public async Task<GalleryItemModel> ToGalleryItemModel(Picture picture, int index)
    {
        var fp = _storageManager.BuildWebpPath(KmApiConsts.MediaTypes.Image, picture.Id);
        var fiUrl = await _storageManager.GetDownloadLink(fp);
        var tp = _storageManager.BuildWebpPath(KmApiConsts.MediaTypes.Thumbnail, picture.Id);
        var tiUrl = await _storageManager.GetDownloadLink(tp);

        return new()
        {
            Alt = picture.AltAttribute,
            FullImage = fiUrl,
            Index = index,
            ThumbImage = tiUrl,
            Title = picture.TitleAttribute,
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
