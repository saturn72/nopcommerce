
using KM.Common.Models.Media;

namespace KM.Navbar.Models;

public record VendorModel
{
    public required int Id { get; init; }
    public string Name { get; init; }
    public GalleryItemModel Picture { get; init; }
}