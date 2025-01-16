﻿
using KM.Common.Models.Media;

namespace KM.Api.Models.Catalog;
public record ProductInfoApiModel
{
    public int Id { get; init; }
    public IEnumerable<ProductBanner> Banners { get; init; }
    public int DisplayIndex { get; init; }
    public string Name { get; init; }
    public decimal? Price { get; init; }
    public string PriceText { get; init; }
    public decimal? PriceOld { get; init; }
    public string PriceOldText { get; init; }
    public decimal? PriceWithDiscount { get; init; }
    public string PriceWithDiscountText { get; init; }
    public string FullDescription { get; init; }
    public IEnumerable<object> Gallery { get; init; }
    public string Gtin { get; init; }
    public string ShortDescription { get; init; }
    public bool ShowStockQuantity { get; init; }
    public string StockAvailability { get; init; }
    public string Mpn { get; init; }
    public string Sku { get; init; }
    public bool ShowOnHomePage { get; init; }
    public string Slug { get; init; }
    public ProductReview Reviews { get; init; }
    public IEnumerable<Variant> Variants { get; init; }

    public record ProductBanner
    {
        public int Priority { get; init; }
        public string Key { get; init; }
    }
    public record ProductReviewRecord
    {
        public int HelpfulNoCount { get; init; }
        public int HelpfulYesCount { get; init; }
        public string Reply { get; init; }
        public string Text { get; init; }
        public string Title { get; init; }
        public string WriterAvatar { get; init; }
        public string WriterName { get; init; }
    }

    public record ProductReview
    {
        public int Rating { get; init; }
        public int TotalReviews { get; init; }
        public IEnumerable<ProductReviewRecord> Records { get; init; }
    }
    public record Variant
    {
        public int Id { get; init; }
        public string ControlType { get; init; }
        public string DefaultValue { get; init; }
        public string Description { get; init; }
        public bool HasCondition { get; init; }
        public bool Required { get; init; }
        public string Name { get; init; }
        public IList<Option> Options { get; init; }
        public string TextPrompt { get; init; }
        public record Option
        {
            public int Id { get; init; }
            public string ColorSquaresRgb { get; init; }
            public bool CustomerEntersQty { get; init; }
            public string DisplayText { get; init; }
            public GalleryItemModel Image { get; init; }
            public int Quantity { get; init; }
            public string Name { get; init; }
            public bool PreSelected { get; init; }
            public decimal? Price { get; init; }
            public string? PriceText { get; init; }
            public decimal? PriceOld { get; init; }
            public string? PriceOldText { get; init; }
            public decimal? PriceWithDiscount { get; init; }
            public string? PriceWithDiscountText { get; init; }
        }
    }
}
