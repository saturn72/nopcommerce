
//namespace KM.Api.Consumer;
//public class OrderUpdatedEventConsumer :
//    IConsumer<EntityInsertedEvent<KmOrder>>,
//    IConsumer<OrderStatusChangedEvent>
//{
//    private readonly IRepository<KmOrder> _kmOrderRepository;
//    private readonly IOrderDocumentStore _orderStore;
//    private readonly IHubContext<OrderHub> _hub;

//    public OrderUpdatedEventConsumer(
//        IRepository<KmOrder> kmOrderRepository,
//        IOrderDocumentStore orderStore,
//        IHubContext<OrderHub> hub)
//    {
//        _kmOrderRepository = kmOrderRepository;
//        _orderStore = orderStore;
//        _hub = hub;
//    }

//    public async Task HandleEventAsync(EntityInsertedEvent<KmOrder> eventMessage)
//    {
//        await UpdateOrderStatus(eventMessage.Entity, eventMessage.Entity.NopOrder);
//    }

//    public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
//    {
//        var k = from a in _kmOrderRepository.Table
//                where a.NopOrderId == eventMessage.Order.Id
//                select a;
//        var ko = await k?.FirstOrDefaultAsync();
//        if (ko == default)
//            return;

//        await UpdateOrderStatus(ko, eventMessage.Order);
//    }

//    private async Task UpdateOrderStatus(KmOrder kmOrder, Order nopOrder)
//    {
//        var status = nopOrder.OrderStatus.ToString();
//        await _orderStore.UpdateAsync(kmOrder.KmOrderId, new() { { "status", status.ToLower() } });

//        await _hub.Clients.Group(kmOrder.KmUserId).SendAsync("updated", kmOrder.KmOrderId);
//    }
//}