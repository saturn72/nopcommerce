using KM.Api.Models.Media;
using KM.Api.Services.Media;
using Nop.Web.Models.Media;

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

    public Task<string> ToThumbnail(int pictureId)
    {
        var tp = _storageManager.BuildWebpPath(KmApiConsts.MediaTypes.Thumbnail, pictureId);
        return _storageManager.GetDownloadLink(tp);
    }

    public Task<string> ToImage(int pictureId)
    {
        var fp = _storageManager.BuildWebpPath(KmApiConsts.MediaTypes.Image, pictureId);
        return _storageManager.GetDownloadLink(fp);
    }
    public async Task<GalleryItemModel> ToGalleryItemModel(Picture picture, int index)
    {
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
