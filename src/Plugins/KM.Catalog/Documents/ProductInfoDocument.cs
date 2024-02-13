namespace Km.Catalog.Documents;

public record ProductInfoDocument
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string ProductType { get; init; }
    public int ParentGroupedProductId { get; init; }
    public bool VisibleIndividually { get; init; }
    public string ShortDescription { get; init; }
    public string FullDescription { get; init; }
    public int Rating { get; init; }
    public int Reviews { get; init; }
    public string Sku { get; init; }
    public string Mpn { get; init; }
    public string Gtin { get; init; }
    public bool IsShipEnabled { get; init; }
    public float ShippingCost { get; init; }
    public int Quantity { get; init; }
    public int OrderMinimumQuantity { get; init; }
    public int OrderMaximumQuantity { get; init; }
    public string AllowedQuantities { get; init; }
    public float Price { get; init; }
    public float OldPrice { get; init; }
    public bool IsNew { get; init; }
    public float Weight { get; init; }
    public float Length { get; init; }
    public float Width { get; init; }
    public float Height { get; init; }
    public int DisplayOrder { get; init; }
    public string Slug { get; init; }
    public IEnumerable<string> StructuredData { get; internal set; }
    public VendorInfo Vendor { get; set; }
    public IEnumerable<CatalogMediaInfo> Media { get; init; }
    public IEnumerable<ManufacturerInfo> Manufacturers { get; init; }
    public IEnumerable<string> Tags { get; init; }
    public IEnumerable<TierPriceDocument> TierPrices { get; init; }
    public IEnumerable<string> Categories { get; init; }
}
