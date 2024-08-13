using Nop.Core.Domain.Media;
using Nop.Services.Media;

namespace KM.Api.Controllers;
public static class EntityExtensions
{
    public static async Task<object> ToMediaItemAsync(this Picture picture, int displayOrder = 0)
    {
        if (picture == null)
            return null;

        var (url, _) = await EngineContext.Current.Resolve<IPictureService>().GetPictureUrlAsync(picture);

        return new
        {
            alt = picture.AltAttribute,
            type = "image",
            index = displayOrder,
            url = url,
            mimeType = picture.MimeType,
#warning - optimize for thumb
            thumb = picture.VirtualPath,
            seoFilename = picture.SeoFilename,
            title = picture.TitleAttribute,
        };
    }

    public static object ToMediaItem(this Video video, int displayOrder = 0)
    {
        return new
        {
            type = "video",
            index = displayOrder,
            url = video.VideoUrl,
        };
    }
}
