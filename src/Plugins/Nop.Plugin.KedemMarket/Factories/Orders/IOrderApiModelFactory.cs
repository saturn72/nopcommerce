namespace KedemMarket.Factories.Orders;
public interface IOrderApiModelFactory
{
    Task<OrderInfoModel> PrepareOrderDetailsModelAsync(Order order);
    Task<IEnumerable<OrderInfoModel>> PrepareOrderDetailsModelsAsync(IEnumerable<Order> orders);
}
