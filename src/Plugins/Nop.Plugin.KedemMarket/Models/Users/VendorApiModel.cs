using KedemMarket.Models.Directory;
using KedemMarket.Models.Media;

namespace KedemMarket.Models.Users;
public record VendorApiModel
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public ContactInfoModel ContactInfo { get; init; }
    public int DisplayOrder { get; init; }
    public string MetaKeywords { get; init; }
    public string MetaDescription { get; init; }
    public string MetaTitle { get; init; }
    public GalleryItemModel? Image { get; init; }
}
