using Google.Cloud.Firestore;

namespace KM.Catalog.Documents;

[FirestoreData]
public record ProductInfoDocument
{
    [FirestoreProperty("id")]
    public string Id { get; init; }

    [FirestoreProperty("name")]
    public string Name { get; init; }

    [FirestoreProperty("productType")]
    public string ProductType { get; init; }

    [FirestoreProperty("parentGroupedProductId")]
    public int ParentGroupedProductId { get; init; }

    [FirestoreProperty("visibleIndividually")]
    public bool VisibleIndividually { get; init; }

    [FirestoreProperty("shortDescription")]
    public string ShortDescription { get; init; }

    [FirestoreProperty("fullDescription")]
    public string FullDescription { get; init; }

    [FirestoreProperty("rating")]
    public int Rating { get; init; }

    [FirestoreProperty("reviews")]
    public int Reviews { get; init; }

    [FirestoreProperty("sku")]
    public string Sku { get; init; }

    [FirestoreProperty("mpn")]
    public string Mpn { get; init; }

    [FirestoreProperty("gtin")]
    public string Gtin { get; init; }

    [FirestoreProperty("isShipEnabled")]
    public bool IsShipEnabled { get; init; }

    [FirestoreProperty("shippingCost")]
    public float ShippingCost { get; init; }

    [FirestoreProperty("quantity")]
    public int Quantity { get; init; }

    [FirestoreProperty("orderMinimumQuantity")]
    public int OrderMinimumQuantity { get; init; }

    [FirestoreProperty("orderMaximumQuantity")]
    public int OrderMaximumQuantity { get; init; }

    [FirestoreProperty("allowedQuantities")]
    public string AllowedQuantities { get; init; }

    [FirestoreProperty("price")]
    public float Price { get; init; }

    [FirestoreProperty("oldPrice")]
    public float OldPrice { get; init; }

    [FirestoreProperty("isNew")]
    public bool IsNew { get; init; }

    [FirestoreProperty("weigth")]
    public float Weight { get; init; }

    [FirestoreProperty("length")]
    public float Length { get; init; }

    [FirestoreProperty("width")]
    public float Width { get; init; }

    [FirestoreProperty("height")]
    public float Height { get; init; }

    [FirestoreProperty("displayOrder")]
    public int DisplayOrder { get; init; }

    [FirestoreProperty("slug")]
    public string Slug { get; init; }

    [FirestoreProperty("structuredData")]
    public IEnumerable<string> StructuredData { get; init; }

    [FirestoreProperty("vendor")]
    public VendorInfo Vendor { get; set; }

    [FirestoreProperty("media")]
    public IEnumerable<CatalogMediaInfo> Media { get; init; }

    [FirestoreProperty("manufacturers")]
    public IEnumerable<ManufacturerInfo> Manufacturers { get; init; }

    [FirestoreProperty("tags")]
    public IEnumerable<string> Tags { get; init; }

    [FirestoreProperty("tierPrices")]
    public IEnumerable<TierPriceDocument> TierPrices { get; init; }

    [FirestoreProperty("categories")]
    public IEnumerable<string> Categories { get; init; }
}

