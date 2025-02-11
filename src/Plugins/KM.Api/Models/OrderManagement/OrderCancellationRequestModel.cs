namespace KedemMarket.Api.Models.OrderManagement;

public record OrderCancellationRequestModel
{
    public int OrderId{ get; set; }
    public string CancellationReason{ get; set; }
}
