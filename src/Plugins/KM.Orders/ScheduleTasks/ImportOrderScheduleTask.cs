

namespace KM.Orders.ScheduleTasks
{
    public class ImportOrderScheduleTask : IScheduleTask
    {
        private const string DefaultPaymentMethod = "cash";
        private static object ImportOrderLock = new();

        #region fields
        private readonly IKmOrderService _kmOrderService;
        private readonly IOrderDocumentStore _orderStore;
        private readonly IRepository<KmOrder> _kmOrderRepository;
        private readonly ILogger _logger;

        #endregion

        #region ctor
        public ImportOrderScheduleTask(
            IKmOrderService kmOrderService,
            IOrderDocumentStore orderStore,
            IRepository<KmOrder> kmOrderRepository,
            ILogger logger)
        {
            _kmOrderService = kmOrderService;
            _orderStore = orderStore;
            _kmOrderRepository = kmOrderRepository;
            _logger = logger;
        }

        #endregion

        public async Task ExecuteAsync()
        {
            await _logger.InformationAsync($"Starting {nameof(ImportOrderScheduleTask)} execution");

            List<string> existIds;
            lock (ImportOrderLock)
            {
                existIds = _kmOrderRepository.Table?.Select(c => c.KmOrderId)?.ToList() ?? new();
            }

            var offset = 0;
            var pageSize = 100;
            var totalOrders = 0;
            var newOrders = Enumerable.Empty<FirestoreCartDocument>();

            var ids = string.Join(",", existIds);
            do
            {
                await _logger.InformationAsync($"Getting store's (firebase) exist orders with parameters: {nameof(existIds)}: {ids}, {nameof(pageSize)}:{pageSize}, {nameof(offset)}: {offset}");
                newOrders = await _orderStore.GetNewOrderPageAsync(
                    pageSize: pageSize,
                    offset: offset);

                totalOrders += newOrders.Count();
                var requests = new List<CreateOrderRequest>();
                foreach (var no in newOrders)
                {
                    lock (ImportOrderLock)
                    {
                        if (existIds.Contains(no.id))
                        {
                            _logger.Warning($"Order with Id:\'{no.id}\' already exists - skipping");
                            continue;
                        }
                        existIds.Add(no.id);
                    }

                    requests.Add(new CreateOrderRequest
                    {
                        KmOrderId = no.id,
                        KmUserId = no.userId,
                        StoreId = no.storeId,
                        CartItems = ToCartItems(no),
                        PaymentMethod = (no.paymentMethod ?? DefaultPaymentMethod).ToSystemPaymentMethod(),
                        BillingInfo = ToAddress(no.user.billingInfo),
                        ShippingAddress = ToAddress(no.user.shippingAddress),
                    });

                    _ = await _kmOrderService.CreateOrdersAsync(requests);
                }

                offset += pageSize;
            }
            while (newOrders.Count() == pageSize);

            await _logger.InformationAsync($"Finish import KM orders. total orders imported = {totalOrders}");
        }

        private static Address ToAddress(AddressDocument document)
        {
            if (document == null)
                return null;

            var names = document.fullName.Split(' ');
            return new()
            {
                Address1 = document.address,
                City = document.city,
                Email = document.email,
                FirstName = names.Length >= 0 ? names[0] : null,
                LastName = names.Length > 0 ? names[1] : null,
                PhoneNumber = document.phoneNumber,
            };
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
