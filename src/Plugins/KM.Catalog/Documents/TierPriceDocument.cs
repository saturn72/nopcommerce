namespace KM.Catalog.Documents;

public record TierPriceDocument
{
    public int Quantity { get; set; }
    public float Price { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
}
