
namespace KM.Orders.ScheduleTasks
{
    public class ImportOrderScheduleTask : IScheduleTask
    {
        private const string DefaultPaymentMethod = "cash";

        #region fields
        private readonly IKmOrderService _kmOrderService;
        private readonly IDocumentStore<FirestoreCartDocument> _orderStore;
        private readonly IRepository<KmOrder> _kmOrderRepository;
        #endregion

        #region ctor
        public ImportOrderScheduleTask(
            IKmOrderService kmOrderService,
            IDocumentStore<FirestoreCartDocument> orderStore,
            IRepository<KmOrder> kmOrderRepository)
        {
            _kmOrderService = kmOrderService;
            _orderStore = orderStore;
            _kmOrderRepository = kmOrderRepository;
        }

        #endregion


        public async Task ExecuteAsync()
        {
            var l = _kmOrderRepository.Table
                         .OrderByDescending(c => c.CreatedOnUtc)
                         .FirstOrDefault();

            var lastImportedOrderId = l?.KmOrderId;

            var offset = 0;
            var pageSize = 100;
            var newOrders = Enumerable.Empty<FirestoreCartDocument>();
            do
            {
                newOrders = await _orderStore.GetPageAsync(
                    afterIndex: lastImportedOrderId,
                    pageSize: pageSize,
                    offset: offset);

                var requests = new List<CreateOrderRequest>();
                foreach (var no in newOrders)
                {
                    requests.Add(new CreateOrderRequest
                    {
                        KmOrderId = no.Id,
                        KmUserId = no.userId,
                        StoreId = no.storeId,
                        CartItems = ToCartItems(no),
                        PaymentMethod = (no.paymentMethod ?? DefaultPaymentMethod).ToSystemPaymentMethod(),
                    });

                    _ = await _kmOrderService.CreateOrdersAsync(requests);
                }

                offset += pageSize;
            }
            while (newOrders.Count() == pageSize);
        }

        private IEnumerable<ShoppingCartItem> ToCartItems(FirestoreCartDocument fcd)
        {
            var scis = new List<ShoppingCartItem>();
            foreach (var item in fcd.items)
            {
                if (!int.TryParse(item.product.id, out var pId))
                    continue;
                var sci = new ShoppingCartItem
                {
                    ProductId = pId,
                    Quantity = item.orderedQuantity,
                    CustomerEnteredPrice = fcd.customerEnteredPrice,
                };
                scis.Add(sci);
            };
            return scis;
        }
    }
}
