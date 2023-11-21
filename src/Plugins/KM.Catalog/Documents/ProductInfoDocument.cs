namespace Km.Catalog.Documents;

[FirestoreData]
public record ProductInfoDocument : IDocument
{
    [FirestoreProperty]
    public string id { get; init; }
    [FirestoreProperty]
    public string name { get; init; }
    [FirestoreProperty]
    public string productType { get; init; }
    [FirestoreProperty]
    public int parentGroupedProductId { get; init; }
    [FirestoreProperty]
    public bool visibleIndividually { get; init; }
    [FirestoreProperty]
    public string shortDescription { get; init; }
    [FirestoreProperty]
    public string fullDescription { get; init; }
    [FirestoreProperty]
    public int rating { get; init; }
    [FirestoreProperty]
    public int reviews { get; init; }
    [FirestoreProperty]
    public string sku { get; init; }
    [FirestoreProperty]
    public string mpn { get; init; }
    [FirestoreProperty]
    public string gtin { get; init; }
    [FirestoreProperty]
    public bool isShipEnabled { get; init; }
    [FirestoreProperty]
    public float shippingCost { get; init; }
    [FirestoreProperty]
    public int quantity { get; init; }
    [FirestoreProperty]
    public int orderMinimumQuantity { get; init; }
    [FirestoreProperty]
    public int orderMaximumQuantity { get; init; }
    [FirestoreProperty]
    public string allowedQuantities { get; init; }
    [FirestoreProperty]
    public float price { get; init; }
    [FirestoreProperty]
    public float oldPrice { get; init; }
    [FirestoreProperty]
    public bool isNew { get; init; }
    [FirestoreProperty]
    public float weight { get; init; }
    [FirestoreProperty]
    public float length { get; init; }
    [FirestoreProperty]
    public float width { get; init; }
    [FirestoreProperty]
    public float height { get; init; }
    [FirestoreProperty]
    public int displayOrder { get; init; }
    [FirestoreProperty]
    public IEnumerable<CatalogMediaInfo> media { get; init; }
    [FirestoreProperty]
    public IEnumerable<ManufacturerInfo> manufacturers { get; init; }
    [FirestoreProperty]
    public IEnumerable<string> tags { get; init; }
    [FirestoreProperty]
    public IEnumerable<TierPriceDocument> tierPrices { get; init; }
    [FirestoreProperty]
    public IEnumerable<string> categories { get; init; }
}
