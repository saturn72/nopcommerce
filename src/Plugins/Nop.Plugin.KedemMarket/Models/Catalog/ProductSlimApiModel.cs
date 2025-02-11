namespace KedemMarket.Models.Catalog;

public record ProductSlimApiModel
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public IEnumerable<ProductInfoApiModel.ProductBanner>? Banners { get; init; }
    public IEnumerable<object>? Gallery { get; init; }
    public decimal? Price { get; init; }
    public string? PriceText { get; init; }
    public decimal? PriceOld { get; init; }
    public string? PriceOldText { get; init; }
    public decimal? PriceWithDiscount { get; init; }
    public string? PriceWithDiscountText { get; init; }
    public IEnumerable<ProductInfoApiModel.Variant>? Variants { get; init; }
    public string? Slug { get; init; }
}
