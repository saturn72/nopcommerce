using ExCSS;

namespace KM.Api.Models.Media;
public record GalleryItemModel
{
    public string Alt { get; init; }
    public string FullImage { get; init; }
    public int Height { get; init; }
    public int Index { get; init; }
    public string ThumbImage { get; init; }
    public string Title { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public int Width { get; init; }
}
