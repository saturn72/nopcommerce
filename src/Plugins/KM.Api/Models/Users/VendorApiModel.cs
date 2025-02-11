using KedemMarket.Api.Models.Directory;
using KedemMarket.Common.Models.Media;

namespace KedemMarket.Api.Models.User;
public record VendorApiModel
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public ContactInfoModel? ContactInfo { get; init; }
    public int DisplayOrder { get; init; }
    public string MetaKeywords { get; init; }
    public string MetaDescription { get; init; }
    public string MetaTitle { get; init; }
    public GalleryItemModel? Image { get; init; }
}
