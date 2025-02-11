using KedemMarket.Models.Catalog;
using KedemMarket.Models.Media;

namespace KedemMarket.Models.Navbar;

public record VendorModel
{
    public required int Id { get; init; }
    public string Name { get; init; }
    public GalleryItemModel Picture { get; init; }
    public string ShortDescription { get; init; }
    public IEnumerable<ProductSlimApiModel> Products { get; init; }
}