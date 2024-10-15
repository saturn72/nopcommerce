using KM.Api.Models.Orders;

namespace KM.Api.Factories;
public interface IOrderApiModelFactory
{
    Task<OrderInfoModel> PrepareOrderDetailsModelAsync(Order order);
}
