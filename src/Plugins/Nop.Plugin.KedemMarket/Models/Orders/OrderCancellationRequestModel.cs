namespace KedemMarket.Models.Orders;

public record OrderCancellationRequestModel
{
    public int OrderId { get; set; }
    public string CancellationReason { get; set; }
}
