using SimpleScheduler;

namespace KM.Catalog.EventConsumers;

public class ProductActivationHandler :
    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityUpdatedEvent<Product>>
{
    private readonly IScheduler _scheduler;
    private readonly IRepository<Product> _productRepository;
    private readonly IOptionsMonitor<SimpleSchedulerOptions> _options;
    private readonly ILogger _logger;

    public ProductActivationHandler(
        IScheduler scheduler,
        IRepository<Product> productRepository,
        IOptionsMonitor<SimpleSchedulerOptions> options,
        ILogger logger)
    {
        _scheduler = scheduler;
        _productRepository = productRepository;
        _options = options;
        _scheduler.AddActivationHandler(ActivationHandler);
        _scheduler.AddErrorHandler(ErrorHandler);
        _logger = logger;
    }

    private Task ErrorHandler(Exception exception, Slot slot)
    {
        return _logger.ErrorAsync(exception.Message, exception);
    }

    private async Task ActivationHandler(Slot slot, int slotsLeft)
    {
        UpdateCatalogTask.EnqueueCatalogUpdateRequest();
        await Task.Yield();

        if (slotsLeft <= _options.CurrentValue.BatchSize)
            _ = SetNextPageAsync(_scheduler.Slots.Last().ExecutedOnUtc);
    }

    internal async Task SetNextPageAsync(DateTime fromDate)
    {
        var activeQuery = _productRepository.Table
            .Where(p => p.AvailableStartDateTimeUtc.HasValue && p.AvailableStartDateTimeUtc >= fromDate)
            .Select(p => new Slot
            {
                ExecutedOnUtc = p.AvailableStartDateTimeUtc.Value,
            }).Take(_options.CurrentValue.BatchSize);

        var deactiveQuery = _productRepository.Table
            .Where(p => p.AvailableEndDateTimeUtc.HasValue && p.AvailableEndDateTimeUtc >= fromDate)
            .Select(p => new Slot
            {
                ExecutedOnUtc = p.AvailableEndDateTimeUtc.Value,
            }).Take(_options.CurrentValue.BatchSize);

        var slots = (await activeQuery.Concat(deactiveQuery)
            .ToListAsync())
            .OrderBy(s => s.ExecutedOnUtc)
            .Take(_options.CurrentValue.BatchSize)
            .ToList();

        _scheduler.AddEntries(slots);
    }

    public Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
    {
        var product = eventMessage.Entity;
        AddScheduledSlots(product);

        return Task.CompletedTask;
    }

    public Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        var product = eventMessage.Entity;
        AddScheduledSlots(product);

        return Task.CompletedTask;
    }
    private void AddScheduledSlots(Product product)
    {
        var slots = new List<Slot>();
        var curDate = DateTime.UtcNow;

        if (product.AvailableStartDateTimeUtc.HasValue &&
            product.AvailableStartDateTimeUtc >= curDate)
        {
            slots.Add(new Slot
            {
                ExecutedOnUtc = product.AvailableStartDateTimeUtc.Value,
            });
        }

        if (product.AvailableEndDateTimeUtc.HasValue &&
            product.AvailableEndDateTimeUtc >= curDate)
        {
            slots.Add(new Slot
            {
                ExecutedOnUtc = product.AvailableEndDateTimeUtc.Value,
            });
        }

        if (slots.Any())
            _scheduler.AddEntries(slots);
    }
}
