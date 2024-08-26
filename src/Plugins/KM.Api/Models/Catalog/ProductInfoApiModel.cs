
namespace KM.Api.Models.Catalog;
public record ProductInfoApiModel
{
    public int Id { get; init; }
    public string Banner { get; init; }
    public string Currency { get; init; }
    public int DisplayIndex { get; init; }
    public string Name { get; init; }
    public decimal CurrentPrice { get; init; }
    public string FullDescription { get; init; }
    public IEnumerable<object> Gallery { get; init; }
    public string Gtin { get; init; }
    public decimal ProductPrice { get; init; }
    public string ShortDescription { get; init; }
    public bool ShowStockQuantity { get; init; }
    public int StockQuantity { get; init; }
    public string Sku { get; init; }
    public string Mpn { get; init; }
    public string Slug { get; init; }
    public IEnumerable<object> Reviews { get; init; }
    public object Variants { get; init; }
}
